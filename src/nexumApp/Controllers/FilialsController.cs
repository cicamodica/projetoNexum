using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nexumApp.Data;
using nexumApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Ong")]
    public class FilialsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FilialsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: FilialsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FilialsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Filial Filial, string Action)
        {
            if (ModelState.IsValid)
            {
                CreateFilial(Filial);
                if(Action == "Finalizar")
                {
                return RedirectToAction("Index", "Home");
                }
                ModelState.Clear();
                ViewBag.Message = "Filial cadastrada com sucesso!";
                return View();
            }
            return View(Filial);
        }
        private void CreateFilial(Filial Filial)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ong = _context.Ongs.FirstOrDefault(ong => ong.UserId == userId);
            Filial.UserId = userId;
            Filial.OngId = ong.Id;
            Filial.Nome = ong.Nome;
            Filial.Descriçao = ong.Descriçao;
            Filial.Endereço = $"{Filial.Complemento} - {Filial.Endereço}";
            _context.Add(Filial);
            _context.SaveChanges();
        }
    }

}
