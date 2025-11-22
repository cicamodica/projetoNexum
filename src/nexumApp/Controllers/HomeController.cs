using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nexumApp.Models;
using System.Diagnostics;
using System.IO;
using System.Linq; 
using System.Security.Claims;         
using System.Threading.Tasks;

namespace nexumApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string? cep, string? cidade, string? tags, [FromQuery] bool publicView = false)
        {
            // O tagId é o índice da tag que vem do formulário de filtro

            // Inicia a consulta base (todas as metas ativas)
            var query = _context.Metas
                .Include(m => m.Ong)
                .Include(m => m.Filial)
                .Include(m => m.Doacoes)
                .Where(m => m.Status == "Ativa");

            if (!string.IsNullOrWhiteSpace(tags))
            {
                var tagIds = tags
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                query = query.Where(m =>
                 m.Ong.Tag.HasValue &&
                  tagIds.Contains(m.Ong.Tag.Value)
   );

                ViewBag.SelectedTags = tagIds;
            }
                  tagIds.Contains(m.Ong.Tag.Value)
            if (User.Identity?.IsAuthenticated == true)
            {
                // Verifica se o usuário logado é uma ONG
                if (User.IsInRole("Ong"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Busca o ID da ONG correspondente ao usuário
                    var ong = await _context.Ongs
                                            .FirstOrDefaultAsync(o => o.UserId == userId);

                    // Define o ViewBag.OngId para o _LoginPartial usar
                    if (ong != null)
                    {
                        ViewBag.OngId = ong.Id;
                    }
                }
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


            // 6. Envia o resultado para a View
            return View(metasPublicas);
        }

        //PESQUISA REDIRECIONADA PARA PÁGINA MARKETPLACE FILTRADA:

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Marketplace(string search, string cep, string cidade, int tagId)
        {
   
            search = search?.Trim();

          
            var query = _context.Metas
                .Include(m => m.Ong)
                .Include(m => m.Filial)
                .Include(m => m.Doacoes)
                .Where(m => m.Status == "Ativa");

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.Recurso.Contains(search) ||
                    m.Descricao.Contains(search) ||
                    m.Ong.Nome.Contains(search) ||
                    m.Ong.Endereço.Contains(search) ||
                    (m.Filial != null && m.Filial.Endereço.Contains(search))
                );
            }

     

            var metasPublicas = await query
                .OrderBy(m => m.DataFim)
                .ToListAsync();


            var pendingValues = new Dictionary<int, int>();
            foreach (var meta in metasPublicas)
            {
                int valorPendente = meta.Doacoes
                    .Where(d => d.Status == "Pendente")
                    .Sum(d => d.Quantidade);

                pendingValues[meta.Id] = valorPendente;
            }

            ViewBag.PendingValues = pendingValues;
            ViewBag.Search = search;
            ViewBag.TagsList = new Tags().TagsList; 

            return View("Index", metasPublicas);
        }

        [AllowAnonymous]
        [HttpGet]

        public async Task<IActionResult> GetVagasPartial(string cep, string cidade, string datas, string search)
        {

            search = search?.Trim();

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

            if (!string.IsNullOrWhiteSpace(search))
            {
                vagasQuery = vagasQuery.Where(v =>
                    v.Titulo.Contains(search) ||
                    v.Descricao.Contains(search) ||
                    v.Ong.Nome.Contains(search) ||
                    v.Ong.Endereço.Contains(search)
                );
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
        [RequestSizeLimit(50 * 1024 * 1024)] // Limite de 50 MB para upload de arquivos
        public async Task<IActionResult> InscreverVoluntario(int id, string nomeCompleto, string email, string telefone, string cpf, DateTime? dataNascimento, string genero, string habilidades, IFormFile foto)
        {
            var vaga = await _context.Vagas.AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
            string fotoUrl = null;
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

            if (foto != null && foto.Length > 0)
            {
                // 1. Define o caminho da pasta
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "candidatos");

                // 2. Cria a pasta se ela não existir
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 3. Cria um nome de arquivo único
                string fileExtension = Path.GetExtension(foto.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 4. Salva o arquivo no disco
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(fileStream);
                }

                // 5. Armazena o URL relativo para o banco de dados
                fotoUrl = $"/uploads/candidatos/{uniqueFileName}";
            }
          
            if (!ModelState.IsValid)
            {
                
                ViewBag.NomeCompleto = nomeCompleto;
                ViewBag.Email = email;
                ViewBag.Telefone = telefone;
                ViewBag.CPF = cpf;
                ViewBag.DataNascimento = dataNascimento;
                ViewBag.Genero = genero;
                ViewBag.Habilidades = habilidades;

                
                if (!string.IsNullOrEmpty(fotoUrl))
                {
                    ViewBag.FotoBase64 = fotoUrl;
                }

                var vagaParaReexibir = await _context.Vagas
                                  .Include(v => v.Ong)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(v => v.IdVaga == id);

                Response.StatusCode = 400;
                return PartialView("_VagaDetalheFormPartial", vagaParaReexibir);
            }




            var novoCandidato = new Candidato
            {
                Nome = nomeCompleto,
                Email = email,
                Telefone = telefone,
                CPF = cpf,
                Descricao = habilidades,
                DataInscricao = DateTime.Now,
                FotoUrl = fotoUrl,


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

                // Em caso de erro no DB, re-populamos o ViewBag (mantendo a foto, se subiu)
                ViewBag.NomeCompleto = nomeCompleto;
                ViewBag.Email = email;
                ViewBag.Telefone = telefone;
                ViewBag.CPF = cpf;
                ViewBag.DataNascimento = dataNascimento;
                ViewBag.Genero = genero;
                ViewBag.Habilidades = habilidades;
                ViewBag.FotoBase64 = fotoUrl; // URL da foto temporária

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

                // Em caso de erro no DB, re-populamos o ViewBag (mantendo a foto, se subiu)
                ViewBag.NomeCompleto = nomeCompleto;
                ViewBag.Email = email;
                ViewBag.Telefone = telefone;
                ViewBag.CPF = cpf;
                ViewBag.DataNascimento = dataNascimento;
                ViewBag.Genero = genero;
                ViewBag.Habilidades = habilidades;
                ViewBag.FotoBase64 = fotoUrl; // URL da foto temporária

                var vagaParaReexibir = await _context.Vagas.Include(v => v.Ong).AsNoTracking().FirstOrDefaultAsync(v => v.IdVaga == id);
                Response.StatusCode = 500;
                return PartialView("_VagaDetalheFormPartial", vagaParaReexibir);
            }
            try // codigo que envia o email 
            {
                string corpoEmail = $@"
                    <div style='font-family: Arial, sans-serif; color: #333;'>
                        <h2 style='color: #16435D;'>Olá, {nomeCompleto}!</h2>
                        <p>Sua inscrição foi realizada com sucesso!</p>
                        <hr>
                        <p><strong>Status atual:</strong> <span style='color: orange;'>Em análise</span></p>
                        <p>A ONG analisará seu perfil e entrará em contato pelo telefone: <strong>{telefone}</strong>.</p>
                        <br>
                        <p>Atenciosamente,<br><strong>Equipe Nexum</strong></p>
                    </div>";

                await _emailService.SendEmailAsync(email, "Confirmação de Inscrição - Nexum", corpoEmail);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "A inscrição foi salva, mas falhou ao enviar o e-mail de confirmação.");
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SmartRedirect()
        {
            //  Verifica se está logado
            if (User.Identity?.IsAuthenticated == true)
            {
                // Pega o objeto User logado
                var user = await _userManager.GetUserAsync(User);

                // Verifica se é uma ONG
                if (user != null && await _userManager.IsInRoleAsync(user, "Ong"))
                {
                    // LOGADO E É ONG -> Vai para o Dashboard
                    return RedirectToAction("Dashboard", "Ongs");
                }
            }

            // NÃO LOGADO (ou logado como outra Role) -> Vai para Home/Index pública
            return RedirectToAction("Index", "Home");
        }



            return RedirectToAction("Index", "Home");
        }



    }
}
