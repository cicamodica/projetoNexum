using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using X.PagedList.Extensions;

namespace nexumApp.Controllers
{
    public class OngsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OngsController(
            ApplicationDbContext context)
        {
            _context = context;   
        }

        // GET: Ongs
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var ongs = await _context.Ongs.Where(ong => ong.Aprovaçao == true).ToListAsync(); 
            var tags = new Tags().TagsNames;
            ViewBag.Tags = tags;
            ViewBag.Total = ongs.Count;
            return View(ongs.ToPagedList(pageNumber, pageSize));
        }



        // OngsController.cs

        [Authorize(Roles = "Ong")] // Só ONGs logadas podem ver
        public async Task<IActionResult> Dashboard()
        {
            // Pega o UserId (string) do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge(); // Não está logado, força login
            }

            // Encontra a entidade ONG, INCLUINDO as Filiais
            var ong = await _context.Ongs
                                    .Include(o => o.Filials)
                                    .FirstOrDefaultAsync(o => o.UserId == userId);

            if (ong == null)
            {
                return NotFound("Nenhuma ONG associada a este usuário.");
            }

            // Verifica a aprovação (do seu modelo Ong.cs)
            if (ong.Aprovaçao == false)
            {
                return RedirectToAction(nameof(Wait)); // Redireciona se não aprovada
            }

            // Busca as metas SOMENTE desta ONG, INCLUINDO a Filial
            var metas = await _context.Metas
                                        .Include(m => m.Ong)
                                        .Include(m => m.Filial)
                                        .Where(m => m.OngId == ong.Id)
                                        .ToListAsync();

            var vagas = await _context.Vagas
                .Where(v => v.IdONG == ong.Id)
                .ToListAsync();

            // Passa os dados extras para a View usando o ViewBag
            ViewBag.Metas = metas;
            ViewBag.Vagas = vagas;

            // NOVO: Passa a lista de filiais e o nome da ONG para a ViewBag
            ViewBag.Filiais = ong.Filials;
            ViewBag.OngNome = ong.Nome;

            // INJEÇÃO CRUCIAL PARA O ÍCONE DE PERFIL NO _LAYOUT.CSHTML
            ViewBag.OngId = ong.Id;

            // Passa a própria ONG como o Modelo principal da View
            return View(ong);
        }

        // GET: Ongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Instância do serviço de Tags para resolver o nome (assumindo que a classe Tags está acessível)
            var tagsService = new nexumApp.Models.Tags();

            // 1. Carregar a ONG, Usuário (para Email), Metas e Vagas, incluindo todas as coleções aninhadas.
            var ong = await _context.Ongs
                .Include(o => o.User)
                .Include(o => o.Metas)
                    .ThenInclude(m => m.Doacoes)  // ESSENCIAL: Para Contagem de Colaboradores/Doadores
                .Include(o => o.Metas)
                    .ThenInclude(m => m.Filial)   // ESSENCIAL: Para Endereço Condicional da Meta
                .Include(o => o.Vagas)
                    //.ThenInclude(v => v.Filial)   // ESSENCIAL: Para Endereço Condicional da Vaga
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ong == null)
            {
                return NotFound();
            }

            // 2. Resolver a Tag
            ViewBag.TagsList = tagsService.TagsList;
            ViewBag.TagName = tagsService.TagsNames.ElementAtOrDefault(ong.Tag ?? -1) ?? "Não Classificado";

            // 3. Preparar as Listas de Metas e Vagas Separadamente (para as abas)

            var metasResumos = new List<object>();
            var vagasResumos = new List<object>();

            // A. Lógica para METAS (Incluindo Progresso e Colaboradores)
            foreach (var meta in ong.Metas.Where(m => m.Status != "Concluída"))
            {
                // Contagem de Doadores (Doações Confirmadas)
                var colaboradores = meta.Doacoes.Count(d => d.Status == "Confirmada");

                // Progresso (para a barra)
                var progressoPerc = (meta.ValorAlvo > 0) ? ((double)meta.ValorAtual / meta.ValorAlvo) * 100 : 0;

                // Endereço (Filial ou Matriz)
                var endereco = meta.Filial?.Endereço ?? ong.Endereço;

                metasResumos.Add(new
                {
                    Tipo = "Meta",
                    Titulo = meta.Recurso,
                    DataCriacao = meta.DataFim.HasValue ? meta.DataFim.Value.ToShortDateString() : "Sem Data",
                    Objetivo = meta.Descricao, // Usando Descrição como Objetivo na View
                    Progresso = progressoPerc.ToString("F0"), // Porcentagem
                    Colaboradores = colaboradores,
                    Endereco = endereco,
                    Id = meta.Id
                });
            }

            // B. Lógica para VAGAS (Incluindo Endereço)
            foreach (var vaga in ong.Vagas.Where(v => v.Status != "Vaga Fechada"))
            {
                //var endereco = vaga.Filial?.Endereço ?? ong.Endereço;
                var endereco = ong.Endereço;

                vagasResumos.Add(new
                {
                    Tipo = "Vaga",
                    Titulo = vaga.Titulo,
                    DataCriacao = vaga.DataInicio.ToShortDateString(),
                    Objetivo = vaga.Descricao,
                    Progresso = vaga.Status, // Status na coluna Progresso
                    Colaboradores = 1, // Placeholder simples para Vagas
                    Endereco = endereco,
                    Id = vaga.IdVaga
                });
            }

            // 4. Injetar as listas separadas na ViewBag
            ViewBag.MetasResumos = metasResumos.OrderByDescending(r => ((dynamic)r).DataCriacao).ToList();
            ViewBag.VagasResumos = vagasResumos.OrderByDescending(r => ((dynamic)r).DataCriacao).ToList();

            // 5. Retorna a View
            return View(ong);
        }

        // GET: Ongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ong = await _context.Ongs.FindAsync(id);
            var ongOwnerId = ong.UserId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ong == null || userId != ongOwnerId)
            {
                return NotFound();
            }
            var viewModel = new OngEditViewModel
            {
                Id = ong.Id,
                Nome = ong.Nome,
                Descriçao = ong.Descriçao,
                Endereço = ong.Endereço,
            };
            return View(viewModel);
        }

        // POST: Ongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descriçao,Endereço")] OngEditViewModel ong)
        {
            var ongToEdit = await _context.Ongs.SingleOrDefaultAsync(dbOng => dbOng.Id == id);
            var ongOwnerId = ongToEdit.UserId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id != ong.Id || userId != ongOwnerId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {      
                ongToEdit.Nome = ong.Nome;
                ongToEdit.Descriçao = ong.Descriçao;
                ongToEdit.Endereço = ong.Endereço;
                try
                {
                    _context.Update(ongToEdit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OngExists(ong.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ong);
        }

        // GET: Ongs/Delete/5
        public async Task<IActionResult> Delete(string UserId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ong = await _context.Ongs
                .FirstOrDefaultAsync(m => m.Id == id);
            var ongOwnerId = ong.UserId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ong == null || userId != ongOwnerId)
            {
                return NotFound();
            }

            return View(ong);
        }

        // POST: Ongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ong = await _context.Ongs.FindAsync(id);
            var ongOwnerId = ong.UserId;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if( ong == null || userId != ongOwnerId)
            {
                return NotFound();
            }
            _context.Ongs.Remove(ong);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Wait()
        {
            return View();
        }
        private bool OngExists(int id)
        {
            return _context.Ongs.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Ong")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDescription(int ongId, string descricao)
        {
            // A variável 'ongId' é o ID da ONG que veio do campo hidden do modal.
            var ong = await _context.Ongs.FindAsync(ongId);

            // 1. Validação de Segurança e Existência da ONG
            if (ong == null)
            {
                TempData["ErrorMessage"] = "ONG não encontrada.";
                return RedirectToAction("Details", new { id = ongId });
            }

            // Verifica se o usuário logado é o proprietário da ONG
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ong.UserId != userId)
            {
                TempData["ErrorMessage"] = "Acesso negado.";
                return Forbid();
            }

            // 2. Atualiza a descrição no modelo
            ong.Descriçao = descricao; // Atualiza a propriedade "Descriçao" (com cedilha)

            // 3. Salva a alteração no banco de dados
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Descrição atualizada com sucesso!";
            }
            catch (Exception ex)
            {
                // Trate erros de banco de dados
                TempData["ErrorMessage"] = "Erro ao salvar a descrição: " + ex.Message;
            }

            // 4. Redireciona de volta para a página de perfil
            return RedirectToAction("Details", new { id = ongId });
        }

        [Authorize(Roles = "Ong")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSingleTag(int ongId, int selectedTagId)
        {
            var ong = await _context.Ongs.FindAsync(ongId);

            // 1. Validação de Segurança
            if (ong == null || ong.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            // 2. Atualiza a Tag (Salva o novo ID)
            ong.Tag = selectedTagId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Área de atuação atualizada com sucesso!";

            return RedirectToAction("Details", new { id = ongId });
        }


    }
}
