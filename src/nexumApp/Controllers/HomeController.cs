using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nexumApp.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; 
using nexumApp.Data;
using System.Linq; 
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.IO;
using nexumApp.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace nexumApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly Tags _tagsService = new Tags();

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string? cep, string? cidade, int? tagId, [FromQuery] bool publicView = false)
        {
            // O tagId é o índice da tag que vem do formulário de filtro

            if (!publicView && User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            // Inicia a consulta base (todas as metas ativas)
            var query = _context.Metas
                .Include(m => m.Ong)
                .Include(m => m.Filial)
                .Include(m => m.Doacoes)
                .Where(m => m.Status == "Ativa");

            // O filtro de aprovação SÓ é aplicado se o usuário estiver logado
            if (User.Identity?.IsAuthenticated == true)
            {
                query = query.Where(m => m.Ong.Aprovaçao == true);
            }

            // NOVO: 1. Adiciona o filtro por Tag
            if (tagId.HasValue && tagId.Value >= 0)
            {
                // Filtra onde a Tag da ONG é igual ao tagId selecionado
                query = query.Where(m => m.Ong.Tag == tagId.Value);
            }

            // 4. Executa a consulta
            var metasPublicas = await query
                .OrderBy(m => m.DataFim)
                .ToListAsync();

            var pendingValues = new Dictionary<int, int>();

            foreach (var meta in metasPublicas)
            {
                int valorPendente = meta.Doacoes
                    .Where(d => d.Status == "Pendente")
                    .Sum(d => d.Quantidade);

                pendingValues.Add(meta.Id, valorPendente);
            }

            ViewBag.PendingValues = pendingValues;

            // NOVO: 2. Resolve e Injeta a lista de Tags para o filtro no Razor (se necessário)
            // Se você tem um dropdown de tags na View, ele precisa dessa lista.
            ViewBag.TagsList = new Tags().TagsList; // Assumindo que você instanciou a classe Tags para pegar a lista.

            if (User.Identity?.IsAuthenticated == true)
            {
                // Garante que o usuário logado é uma ONG
                if (User.IsInRole("Ong"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Busca a ONG
                    var ong = await _context.Ongs
                                            .FirstOrDefaultAsync(o => o.UserId == userId);

                    // Define o ViewBag.OngId para o _Layout.cshtml usar no ícone de perfil
                    if (ong != null)
                    {
                        ViewBag.OngId = ong.Id;
                    }
                }
            }

            // 6. Envia o resultado para a View
            return View(metasPublicas);
        }

        [AllowAnonymous]
        [HttpGet]

        public async Task<IActionResult> GetVagasPartial(string cep, string cidade, string datas)
        {
            var vagasQuery = _context.Vagas
                .Include(v => v.Ong)
                
                .AsNoTracking();

            if (User.Identity?.IsAuthenticated == true)
            {
                vagasQuery = vagasQuery.Where(v => v.Ong.Aprovaçao == true);
            }


            if (!string.IsNullOrEmpty(cidade) && !cidade.Contains("..."))
            {
                var cidadeQuery = cidade.Split('-')[0].Trim();
                vagasQuery = vagasQuery.Where(v => v.Ong.Endereço.Contains(cidadeQuery));
            }


            if (!string.IsNullOrEmpty(cep))
            {
                var cepQuery = cep.Replace("-", "");
                vagasQuery = vagasQuery.Where(v => v.Ong.Endereço.Contains(cepQuery));
            }


            if (!string.IsNullOrEmpty(datas))
            {
                var dateList = new List<DateTime>();
                var dateStrings = datas.Split(',');

                foreach (var dateStr in dateStrings)
                {

                    if (DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
                    {
                        dateList.Add(parsedDate.Date);
                    }
                }

                if (dateList.Any())
                {
                    var minDate = dateList.Min();
                    var maxDate = dateList.Max();



                    vagasQuery = vagasQuery.Where(v => v.DataInicio <= maxDate && v.DataFim >= minDate);
                }
            }


            var vagasPublicas = await vagasQuery
                .OrderBy(v => v.Titulo)
                .ToListAsync();

            return PartialView("_VagasPartial", vagasPublicas);
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVagaDetalheFormPartial(int vagaId)
        {
            var vaga = await _context.Vagas
                                     .Include(v => v.Ong)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(v => v.IdVaga == vagaId);

            if (vaga == null)
            {
                return NotFound("Vaga não encontrada.");
            }

            return PartialView("_VagaDetalheFormPartial", vaga);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InscreverVoluntario(int id, string nomeCompleto, string email, string telefone, string cpf, DateTime? dataNascimento, string genero, string habilidades, IFormFile foto)
        {
            var vaga = await _context.Vagas.AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
            if (vaga == null)
            {
                ModelState.AddModelError("", "A vaga para a qual você tentou se inscrever não existe mais.");
            }

            if (string.IsNullOrEmpty(nomeCompleto))
            {
                ModelState.AddModelError("NomeCompleto", "O nome é obrigatório.");
            }
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("Email", "O e-mail é obrigatório.");
            }

            if (!ModelState.IsValid)
            {
                var vagaParaReexibir = await _context.Vagas
                                                     .Include(v => v.Ong)
                                                     .AsNoTracking()
                                                     .FirstOrDefaultAsync(v => v.IdVaga == id);

                Response.StatusCode = 400;
                return PartialView("_VagaDetalheFormPartial", vagaParaReexibir);
            }


            byte[] fotoBytes = null;
            if (foto != null && foto.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await foto.CopyToAsync(ms);
                    fotoBytes = ms.ToArray();
                }
            }


            var novoCandidato = new Candidato
            {
                Nome = nomeCompleto,
                Email = email,
                Telefone = telefone,
                CPF = cpf,
                Descricao = habilidades,
                DataInscricao = DateTime.Now,
                Foto = fotoBytes,


                IdVoluntario = null
            };


            try
            {
                _context.Candidatos.Add(novoCandidato);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar novo candidato no banco.");
                ModelState.AddModelError("", "Ocorreu um erro inesperado ao salvar seus dados de candidato.");

                var vagaParaReexibir = await _context.Vagas.Include(v => v.Ong).AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
                Response.StatusCode = 500;
                return PartialView("_VagaDetalheFormPartial", vagaParaReexibir);
            }

            var novaInscricao = new Inscricoes
            {
                IdVaga = id,
                IdCandidato = novoCandidato.Id,
                DataInscricao = DateTime.Now,
                Status = "Pendente"
            };

            try
            {

                _context.Inscricoes.Add(novaInscricao);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar inscrição no banco.");
                _context.Candidatos.Remove(novoCandidato);
                await _context.SaveChangesAsync();

                ModelState.AddModelError("", "Ocorreu um erro inesperado ao salvar sua inscrição.");

                var vagaParaReexibir = await _context.Vagas.Include(v => v.Ong).AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
                Response.StatusCode = 500;
                return PartialView("_VagaDetalheFormPartial", vagaParaReexibir);
            }

            return Ok();
        }

        // Ação para exibir o Perfil da ONG
        public async Task<IActionResult> PerfilONG(int id)
        {
            // 1. Carregar a ONG, Usuário (para Email), Metas e Vagas
            var ong = await _context.Ongs
                .Include(o => o.User)
                .Include(o => o.Metas)
                .Include(o => o.Vagas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ong == null)
            {
                return NotFound();
            }

            // 2. Resolver o Nome da Tag (Área de Atuação)
            ViewBag.TagName = _tagsService.TagsNames.ElementAtOrDefault(ong.Tag ?? -1) ?? "Não Classificado";

            // 3. Criar a lista consolidada de Resumos (Metas e Vagas) para a tabela
            var resumos = new List<object>();

            // Adicionar Metas
            foreach (var meta in ong.Metas.Where(m => m.Status != "Finalizada")) // Exibir apenas metas ativas
            {
                resumos.Add(new
                {
                    Tipo = "Meta",
                    Titulo = meta.Recurso,
                    Descricao = meta.Descricao,
                    Status = meta.Status,
                    DataFim = meta.DataFim.HasValue ? meta.DataFim.Value.ToShortDateString() : "Sem Data",
                    Id = meta.Id
                });
            }

            // Adicionar Vagas
            foreach (var vaga in ong.Vagas.Where(v => v.Status != "Finalizada")) // Exibir apenas vagas ativas
            {
                resumos.Add(new
                {
                    Tipo = "Vaga",
                    Titulo = vaga.Titulo,
                    Descricao = vaga.Descricao,
                    Status = vaga.Status,
                    DataFim = vaga.DataFim.ToShortDateString(),
                    Id = vaga.IdVaga
                });
            }

            // Ordenar os resumos por Tipo para facilitar a leitura na tabela
            ViewBag.Resumos = resumos.OrderBy(r => ((dynamic)r).Tipo).ToList();

            // Passa o objeto ONG como Model
            return View(ong);
        }

    }
}