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
