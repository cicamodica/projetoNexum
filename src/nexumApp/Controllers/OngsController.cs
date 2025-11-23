using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using nexumApp.Data;
using nexumApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Extensions;
using static QuestPDF.Helpers.Colors;
using CloudinaryDotNet;



namespace nexumApp.Controllers
{
    public class OngsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly Cloudinary _cloudinary;
        public OngsController(
            ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IWebHostEnvironment hostEnvironment, UserManager<User> userManager, Cloudinary cloudinary)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _cloudinary = cloudinary;

        }

        // ============================================================
        // GET: Ongs (Lista Pública de ONGs)
        // ============================================================
        public async Task<IActionResult> Index(int? page, string ONGTags)
        {
            int pageSize = 40;
            int pageNumber = (page ?? 1);

            // Filtra apenas ONGs aprovadas para o público ver
            var query = _context.Ongs.AsQueryable();

            // Para Admin mostra todas (ONGs ativas (aprovadas) e pausadas)
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(ong => ong.Aprovaçao == true);
            }

            // Lógica de Filtro por Tags
            if (!string.IsNullOrWhiteSpace(ONGTags))
            {
                // Converte a string "1,2,3" em lista de inteiros
                var idsArray = ONGTags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(int.Parse)
                                      .ToList();

                // Filtra onde a Tag da ONG está na lista selecionada
                query = query.Where(ong => ong.Tag.HasValue && idsArray.Contains(ong.Tag.Value));
            }

            var ongs = await query.ToListAsync();

            // ViewBags para os filtros na View
            ViewBag.Tags = new Tags().TagsNames;
            ViewBag.Total = ongs.Count;

            return View(ongs.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id) //ação para admin pausar/reativar ONG
        {
            var ong = await _context.Ongs.FindAsync(id);
            if (ong == null)
                return NotFound();

            // Inverte o status: se estava true (ativa), vira false (pausada)
            ong.Aprovaçao = !ong.Aprovaçao;

            await _context.SaveChangesAsync();

            // Volta para a lista de ONGs
            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // GET: Ongs/Dashboard
        // ============================================================
        [Authorize(Roles = "Ong")]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Identifica a ONG Logada
            var userId = _userManager.GetUserId(User);

            // Nota: Não precisa verificar "Aprovação" ou "Pendências" aqui.
            // O Filtro Global (VerificarPendenciasOngFilter) já fez isso antes de deixar entrar.

            var ong = await _context.Ongs
                .Include(o => o.Filials)
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (ong == null) return NotFound();

            // 2. Busca dados relacionados para o Painel
            var metas = await _context.Metas
                .Include(m => m.Ong)
                .Include(m => m.Filial)
                .Where(m => m.OngId == ong.Id)
                .ToListAsync();

            var vagas = await _context.Vagas
                .Where(v => v.IdONG == ong.Id)
                .ToListAsync();

            // 3. Preenche as ViewBags necessárias para a View e Layout
            ViewBag.Metas = metas;
            ViewBag.Vagas = vagas;
            ViewBag.Filiais = ong.Filials;
            ViewBag.OngNome = ong.Nome;
            ViewBag.OngId = ong.Id; // Importante para o menu _LoginPartial

            return View(ong);
        }

        // GET: Ongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Instância do serviço de Tags para resolver o nome 
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
                    DataFinal = meta.DataFim.HasValue ? meta.DataFim.Value.ToShortDateString() : "Sem Data",
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
            ViewBag.MetasResumos = metasResumos.OrderByDescending(r => ((dynamic)r).DataFinal).ToList();
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
        [Authorize(Roles = "Ong")]
        public async Task<IActionResult> MeuPerfil()
        {
            // Busca o ID do usuário logado
            var userId = _userManager.GetUserId(User);

            // Busca apenas o ID da ONG no banco (rápido)
            var ongId = await _context.Ongs
                                      .Where(o => o.UserId == userId)
                                      .Select(o => o.Id)
                                      .FirstOrDefaultAsync();

            // Se não achou ONG vinculada, joga para a Home
            if (ongId == 0) return RedirectToAction("Index", "Home");

            // Redireciona para a tela de Detalhes com o ID correto
            return RedirectToAction("Details", new { id = ongId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarConteudoSobre(int id, string conteudoSobre)
        {
            var ong = await _context.Ongs.FindAsync(id);

            if (ong == null)
                return NotFound();

            // Atualiza somente o campo correto
            ong.ConteudoSobre = conteudoSobre;

            _context.Ongs.Update(ong);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id, tab = "sobre" });
        }

        // MÉTODO AUXILIAR (Se não estiver definido em outro lugar)
        private static string FormataCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return "";
            var d = new string(cnpj.Where(char.IsDigit).ToArray());
            return d.Length == 14 ? Convert.ToUInt64(d).ToString(@"00\.000\.000\/0000\-00") : cnpj;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OngPdf(string[] columns)
        {
            // 1. SEGURANÇA: IDENTIFICAR O USUÁRIO LOGADO
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // Se o [Authorize] falhar por algum motivo, retorna o erro
                return Unauthorized("Usuário não logado ou ID não encontrado.");
            }

            // 2. BUSCAR A ONG E DADOS RELACIONADOS (SEGURANÇA E PERFORMANCE)
            // Filtro essencial para garantir que a ONG seja a do usuário logado.
            var ong = await _context.Ongs
                .Include(o => o.User)
                .Include(o => o.Filials) // Inclui as filiais
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.UserId == userId); // Filtro de Acesso

            if (ong == null)
            {
                return NotFound("Nenhuma ONG associada a este usuário logado.");
            }

            // 3. BUSCAR METAS E VAGAS APENAS DESTA ONG
            var metas = await _context.Metas
                .AsNoTracking()
                .Where(m => m.OngId == ong.Id)
                .ToListAsync();

            var vagas = await _context.Vagas
                .AsNoTracking()
                .Where(v => v.IdONG == ong.Id)
                .ToListAsync();

            // 4. DEFINIÇÃO DAS COLUNAS (LÓGICA DE FORMATAÇÃO)
            // O Funções Value (lambda) usam as listas 'metas' e 'vagas' filtradas no escopo.
            var allowed = new Dictionary<string, (string Header, Func<Ong, string> Value, float weight)>
            {
                ["nome"] = ("Nome Razão", o => o.Nome ?? "", 2.2f),
                ["descricao"] = ("Descrição", o => o.Descriçao ?? "", 3.0f),
                ["endereco"] = ("Endereço Matriz", o => o.Endereço ?? "", 2.4f),
                ["cnpj"] = ("CNPJ Matriz", o => FormataCnpj(o.CNPJ), 1.4f),
                ["aprovacao"] = ("Aprovação", o => o.Aprovaçao ? "Aprovada" : "Pendente", 1.2f),
                ["email"] = ("E-mail", o => o.User?.Email ?? "", 1.8f),

                ["filiais"] = ("Filiais Cadastradas", o =>
                {
                    if (o.Filials == null || !o.Filials.Any())
                        return "Não possui filiais";

                    var detalhes = o.Filials.Select(f =>
                        $"{f.Nome} – {f.Endereço} (CNPJ {FormataCnpj(f.CNPJ)})");

                    return $"{o.Filials.Count} filiais:\n- " + string.Join("\n- ", detalhes);
                },
                3.0f),

                ["metas"] = ("Metas de Recursos/Doação", o =>
                {
                    if (!metas.Any())
                        return "Nenhuma meta cadastrada";

                    var detalhes = metas.Select(m =>
                        $"{m.Descricao} – {m.Status} (Atual: {m.ValorAtual}, Alvo: {m.ValorAlvo})");

                    return $"{metas.Count} metas:\n- " + string.Join("\n- ", detalhes);
                },
                3.0f),

                ["vagas"] = ("Vagas de Voluntariado", o =>
                {
                    if (!vagas.Any())
                        return "Nenhuma vaga cadastrada";

                    var detalhes = vagas.Select(v =>
                        $"{v.Titulo} – {v.Status}");

                    return $"{vagas.Count} vagas:\n- " + string.Join("\n- ", detalhes);
                },
                3.0f)
            };

            // 5. FILTRAR E VALIDAR COLUNAS
            var specs = (columns ?? Array.Empty<string>())
                .Select(c => c?.Trim().ToLowerInvariant())
                .Where(c => !string.IsNullOrWhiteSpace(c) && allowed.ContainsKey(c))
                .Distinct()
                .Select(c => new { Key = c!, allowed[c!].Header, allowed[c!].Value, allowed[c!].weight })
                .ToList();

            if (specs.Count == 0)
                return BadRequest("Selecione ao menos uma coluna para gerar o relatório.");

            // 6. PREPARAR DADOS PARA O PDF: Apenas a ONG logada
            var data = new List<Ong> { ong };

            // 7. GERAÇÃO DO PDF (QuestPDF)
            var primary = Colors.Blue.Medium;
            var text = Colors.Grey.Darken4;

            byte[] pdf = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(TextStyle.Default.FontFamily("Poppins").FontSize(10).FontColor(text));

                    // Cabeçalho
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Relatório de Detalhes da ONG: {ong.Nome}")
                                .FontSize(18).SemiBold().FontColor(primary);
                            col.Item().Text(txt =>
                            {
                                txt.Span("Gerado em ").Light();
                                txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                            });
                        });
                    });

                    // Conteúdo: Tabela com os dados da ONG
                    page.Content().Element(container =>
                    {
                        container.PaddingTop(10).Table(table =>
                        {
                            // Definição das colunas
                            table.ColumnsDefinition(cols =>
                            {
                                foreach (var s in specs)
                                    cols.RelativeColumn(s.weight);
                            });

                            // Header (Cabeçalho da Tabela)
                            table.Header(header =>
                            {
                                foreach (var s in specs)
                                {
                                    header.Cell().Element(CellHeader).Text(s.Header);
                                }
                                static IContainer CellHeader(IContainer c) =>
                                    c.Background(Colors.Grey.Lighten3).PaddingVertical(6).PaddingHorizontal(8).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).DefaultTextStyle(TextStyle.Default.SemiBold());
                            });

                            // Corpo da Tabela: Loop único para a ONG logada
                            var i = 0;
                            foreach (var o in data)
                            {
                                foreach (var s in specs)
                                {
                                    table.Cell().Element(e => CellBody(e, i)).Text(s.Value(o));
                                }
                                i++;
                            }

                            static IContainer CellBody(IContainer c, int rowIndex)
                            {
                                var zebra = rowIndex % 2 == 1 ? Colors.Grey.Lighten5 : Colors.White;
                                return c.Background(zebra).PaddingVertical(5).PaddingHorizontal(8).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                            }
                        });
                    });

                    // Rodapé
                    page.Footer().AlignRight().Text(t =>
                    {
                        t.Span("Página ").Light();
                        t.CurrentPageNumber();
                        t.Span(" / ").Light();
                        t.TotalPages();
                    });
                });
            }).GeneratePdf();

            // 8. RETORNO
            return File(pdf, "application/pdf", $"relatorio-ong-{ong.Nome.Replace(" ", "-")}.pdf");
        }

        [HttpPost]
        [Authorize(Roles = "Ong")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(int ongId, IFormFile logoFile)
        {
            var userId = _userManager.GetUserId(User);
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId && o.UserId == userId);

            if (ong == null) return NotFound();

            if (logoFile != null)
            {
                try
                {
                    // Chama o método auxiliar do Cloudinary
                    string urlCloudinary = await UploadToCloudinary(logoFile);

                    if (!string.IsNullOrEmpty(urlCloudinary))
                    {
                        ong.ImageURL = urlCloudinary; // Salva a URL do Cloudinary no banco

                        _context.Update(ong);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Logo atualizada com sucesso!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erro ao enviar imagem: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Details), new { id = ongId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadHeaderImages(int ongId, List<IFormFile> headerImages)
        {
            var userId = _userManager.GetUserId(User);
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId && o.UserId == userId);

            if (ong == null) return NotFound();

            // Pega o primeiro arquivo da lista (já que é single header)
            var file = headerImages.FirstOrDefault();

            if (file != null && file.Length > 0)
            {
                try
                {
                    // Chama o método auxiliar do Cloudinary
                    string urlCloudinary = await UploadToCloudinary(file);

                    if (!string.IsNullOrEmpty(urlCloudinary))
                    {
                        ong.HeaderImageURL = urlCloudinary; // Salva a URL do Cloudinary no banco

                        _context.Update(ong);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Imagem de capa atualizada com sucesso!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erro ao enviar imagem: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Details), new { id = ongId });
        }



        // ============================================================
        // POST: Ongs/UpdatePix
        // Ação para atualizar ou cadastrar a Chave PIX
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ong")] // Garante que só ONGs logadas acessem
        public async Task<IActionResult> UpdatePix(int ongId, string chavePix)
        {
            // 1. Identifica o usuário logado para segurança
            var userId = _userManager.GetUserId(User);

            // 2. Busca a ONG no banco
            // A condição (o.UserId == userId) impede que uma ONG altere o PIX de outra
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId && o.UserId == userId);

            if (ong == null)
            {
                return NotFound();
            }

            // 3. Atualiza o valor
            // O ?.Trim() remove espaços em branco acidentais no início ou fim
            ong.ChavePix = chavePix?.Trim();

            try
            {
                _context.Update(ong);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Chave PIX atualizada com sucesso!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro ao salvar a chave PIX.";
            }

            // 4. Redireciona de volta para a tela de detalhes (Perfil)
            return RedirectToAction(nameof(Details), new { id = ongId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Ong")]
        public async Task<IActionResult> UpdateWebsite(int ongId, string website)
        {
            var userId = _userManager.GetUserId(User);
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId && o.UserId == userId);

            if (ong == null) return NotFound();

            // Adiciona https:// se o usuário esqueceu
            if (!string.IsNullOrEmpty(website) && !website.StartsWith("http"))
            {
                website = "https://" + website;
            }

            ong.Website = website?.Trim();

            try
            {
                _context.Update(ong);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Website atualizado com sucesso!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao salvar o website.";
            }

            return RedirectToAction(nameof(Details), new { id = ongId });
        }

        private async Task<string> UploadToCloudinary(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, ms),
                    UseFilename = true,
                    UniqueFilename = true, // Mudei para true para evitar cache de imagem antiga
                    Overwrite = true,
                    // Format = "jpg" // Opcional: Se quiser forçar JPG. Se remover, mantém o original (png, etc)
                };

                // Faz o upload
                var uploadResult = _cloudinary.Upload(uploadParams);

                // Retorna a URL segura (HTTPS)
                return uploadResult.SecureUrl.ToString();
            }
        }
    }
}




