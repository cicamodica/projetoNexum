using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Data.Migrations;
using nexumApp.Models;
using nexumApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace nexumApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Tags _tagsService = new Tags();
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IEmailService emailService, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string cep, string tags, string search)
        {
            search = search?.Trim();

            // =================================================================
            // 3. CARREGAMENTO DO MARKETPLACE
            // =================================================================

            var query = _context.Metas
                .Include(m => m.Ong)
                .Include(m => m.Filial)
                .Include(m => m.Doacoes)
                .Where(m => m.Status == "Ativa" && m.Ong.Aprovaçao).ToList();

            // --- Filtros ---

            if (search != null)
            {
                query = [.. query.Where(m =>
                    m.Recurso.ToLower().Contains(search.ToLower()) ||
                    m.Descricao.ToLower().Contains(search.ToLower()) ||
                    m.Ong.Nome.ToLower().Contains(search.ToLower())
                )];
            }

            if (tags != null)
            {
                int?[] idsArray = [.. tags.Split(',').Select(int.Parse)];
                query = [.. query.Where(meta => idsArray.Contains(meta.Ong.Tag))];
            }

            if (!string.IsNullOrEmpty(cep))
            {
                var formattedCep = cep.Replace("-", "");
                query = [.. query.Where(meta => meta.Ong.CEP == formattedCep || meta.Filial?.CEP == formattedCep) ];
            }

            // --- Execução ---

            var metasPublicas = query
                .OrderBy(m => m.DataFim);
              
            // --- Cálculo de Pendências ---

            var pendingValues = new Dictionary<int, int>();
            foreach (var meta in metasPublicas)
            {
                int valorPendente = meta.Doacoes
                    .Where(d => d.Status == "Pendente")
                    .Sum(d => d.Quantidade);

                pendingValues.Add(meta.Id, valorPendente);
            }

            ViewBag.PendingValues = pendingValues;
            return View(metasPublicas);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetVagasPartial(string cep, string tags, string search)
        { 
            var vagasFromDb = _context.Vagas.Include(vaga => vaga.Ong).Where(vaga => vaga.Ong.Aprovaçao).ToList();

            search = search?.Trim();

            if(search != null)
            {
                vagasFromDb = vagasFromDb.Where(vaga =>
                vaga.Titulo.ToLower().Contains(search.ToLower()) ||
                vaga.Descricao.ToLower().Contains(search.ToLower()) ||
                vaga.Ong.Nome.ToLower().Contains(search.ToLower())
                ).ToList();
            }


            if (tags != null)
            {
                int?[] idsArray = [.. tags.Split(',').Select(int.Parse)];
                var ongs = _context.Ongs.Where(ong => idsArray.Contains(ong.Tag)).ToList();
                var ongsIds = ongs.Select(ong => ong.Id).ToArray();
                vagasFromDb = [.. vagasFromDb.Where(vaga => ongsIds.Contains(vaga.IdONG))];
            }

            if (!string.IsNullOrEmpty(cep))
            {
                var formattedCep = cep.Replace("-", "");
                var ongs = _context.Ongs.Where(ong => ong.CEP == formattedCep).ToList();
                var ongsIds = ongs.Select(ong => ong.Id).ToArray();
                vagasFromDb = [.. vagasFromDb.Where(vaga => ongsIds.Contains(vaga.IdONG))];
            }

            return PartialView("_VagasPartial", vagasFromDb);
        }




        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetVagaDetalheFormPartial(int vagaId)
        {
            var vaga = await _context.Vagas
                                     .Include(v => v.Ong)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(v => v.IdVaga == vagaId && v.Ong.Aprovaçao);

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
            var vaga = await _context.Vagas
                .Include(v => v.Ong)
                .ThenInclude(o => o.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.IdVaga == id);

            


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

            if (string.IsNullOrWhiteSpace(cpf))
            {
                ModelState.AddModelError("CPF", "O CPF é obrigatório.");
            }
            else
            {
                var cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());

                if (cpfLimpo.Length != 11)
                {
                    ModelState.AddModelError("CPF", "O CPF deve conter 11 dígitos.");
                }
            }

            if (dataNascimento == null)
            {
                ModelState.AddModelError("DataNascimento", "A data de nascimento é obrigatória.");
            }
            else
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - dataNascimento.Value.Year;

                if (dataNascimento.Value.Date > hoje.AddYears(-idade))
                    idade--;

                if (idade < 18)
                {
                    ModelState.AddModelError("", "É necessário ter 18 anos ou mais para se inscrever como voluntário.");
                }
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




            var novoCandidato = new Models.Candidato
            {
                Nome = nomeCompleto,
                Email = email,
                Telefone = telefone,
                CPF = cpf,
                Descricao = habilidades,
                DataInscricao = DateTime.Now,
                FotoUrl = fotoUrl,


                //IdVoluntario = null
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

            try
            {
                
                if (vaga?.Ong?.User == null || string.IsNullOrEmpty(vaga.Ong.User.Email))
                {
                    _logger.LogWarning(
                        "E-mail da ONG não encontrado. VagaId: {VagaId} | OngId: {OngId} | UserId: {UserId}",
                        vaga?.IdVaga,
                        vaga?.IdONG,
                        vaga?.Ong?.UserId
                    );
                }
                else
                {
                    string ongEmail = vaga.Ong.User.Email;

                    string corpoEmailOng = $@"
                           <div style='font-family: Arial, sans-serif; color: #333;'>
                           <h2>Nova Inscrição para a Vaga: {vaga.Titulo}</h2>
                           <p>Um novo voluntário se inscreveu para a vaga que você publicou.</p>
                           <hr>
                           <p><strong>Nome do Candidato:</strong> {nomeCompleto}</p>
                           <p><strong>E-mail:</strong> {email}</p>
                           <p><strong>Telefone:</strong> {telefone}</p>
                           <p><strong>CPF:</strong> {cpf}</p>
                           <p><strong>Data de Nascimento:</strong> {dataNascimento?.ToString("dd/MM/yyyy")}</p>
                           <p><strong>Habilidades/Descrição:</strong> {habilidades}</p>
                                <br>
                               <p>Atenciosamente,<br><strong>Equipe Nexum</strong></p>
                          </div>";

                    await _emailService.SendEmailAsync(
                        ongEmail,
                        "Novo Voluntário Inscrito - Nexum",
                        corpoEmailOng
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falhou ao enviar o e-mail para a ONG.");
            }

            return Ok();
        }

        public IActionResult SmartRedirect()
        {
            if (User.IsInRole("Ong")) return RedirectToAction("Dashboard", "Ongs");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> QuemSomos()
        {
            return View();
        }
    }
}