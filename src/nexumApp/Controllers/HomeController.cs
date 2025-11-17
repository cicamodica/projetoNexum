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

namespace nexumApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] bool publicView = false)
        {
            
            if (!publicView && User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            //Inicia a consulta base (todas as metas ativas)
            var query = _context.Metas
                .Include(m => m.Ong)
                .Where(m => m.Status == "Ativa");

            
            // O filtro de aprovação SÓ é aplicado se o usuário estiver logado
            // (e não for um Admin, pois ele já foi redirecionado).
            if (User.Identity?.IsAuthenticated == true)
            {
                query = query.Where(m => m.Ong.Aprovaçao == true);
            }
            // Se o usuário NÃO estiver logado (anônimo), o filtro de aprovação é pulado.

            // 4. Executa a consulta
            var metasPublicas = await query
                .OrderBy(m => m.DataFim)
                .ToListAsync();

            // 5. Envia o resultado para a View
            return View(metasPublicas);
        }

        [AllowAnonymous]
        [HttpGet]
        
        public async Task<IActionResult> GetVagasPartial(string cep, string cidade, string datas)
        {
            var vagasQuery = _context.Vagas
                .Include(v => v.Ong)
                //.Where(v => v.Status == "Ativa")
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

       


        public IActionResult About()
        {
            return View();
        }

        public IActionResult QuemSomos()
        {
            return View();
        }

        public IActionResult FaleConosco()
        {
            return View();
        }

        public IActionResult Feedback()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}