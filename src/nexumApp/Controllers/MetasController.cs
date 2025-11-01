using System;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Ong")] // SÓ ONGs logadas podem acessar
    public class MetasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager; 

        // MODIFICAR o construtor
        public MetasController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        
        // Pega o Id (int) da ONG com base no UserId (string) do usuário logado
        private async Task<int?> GetOngIdLogadaAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return null;

            var ong = await _context.Ongs
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(o => o.UserId == userId);

            return ong?.Id;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Ongs");
        }


        // GET: Metas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ongId = await GetOngIdLogadaAsync();
            var meta = await _context.Metas
                .Include(m => m.Ong)
                .FirstOrDefaultAsync(m => m.Id == id);

            // VALIDAÇÃO DE SEGURANÇA:
            if (meta == null || meta.OngId != ongId)
            {
                return Forbid();
            }

            return View(meta);
        }

        // GET: Metas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Metas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Recurso,ValorAlvo")] Meta meta)
        {

            var ongId = await GetOngIdLogadaAsync();
            if (ongId == null) return Forbid();

            meta.OngId = ongId.Value;
            meta.Status = "Ativa";
            meta.ValorAtual = 0;
            meta.QuantidadeReservada = 0;

            if (ModelState.IsValid)
            {
                _context.Add(meta);
                await _context.SaveChangesAsync();

                // Redireciona para o Dashboard
                return RedirectToAction("Dashboard", "Ongs");
            }

            return View(meta);
        }

        // GET: Metas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ongId = await GetOngIdLogadaAsync();
            var meta = await _context.Metas.FindAsync(id);

            // VALIDAÇÃO DE SEGURANÇA:
            if (meta == null || meta.OngId != ongId)
            {
                return Forbid();
            }

            return View(meta);
        }

        // POST: Metas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Recurso,ValorAlvo,Status")] Meta meta)
        {
            if (id != meta.Id) return NotFound();

            var ongId = await GetOngIdLogadaAsync();
            var metaOriginal = await _context.Metas
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(m => m.Id == id);

            if (metaOriginal == null || metaOriginal.OngId != ongId)
            {
                return Forbid();
            }

            meta.OngId = metaOriginal.OngId;
            meta.ValorAtual = metaOriginal.ValorAtual;
            meta.QuantidadeReservada = metaOriginal.QuantidadeReservada;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // ... (código de erro)
                }
                // Redireciona para o Dashboard
                return RedirectToAction("Dashboard", "Ongs");
            }
            return View(meta);
        }

        // GET: Metas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ongId = await GetOngIdLogadaAsync();
            var meta = await _context.Metas
                .Include(m => m.Ong)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meta == null || meta.OngId != ongId)
            {
                return Forbid();
            }

            return View(meta);
        }

        // POST: Metas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ongId = await GetOngIdLogadaAsync();
            var meta = await _context.Metas.FindAsync(id);

            if (meta == null || meta.OngId != ongId)
            {
                return Forbid();
            }

            _context.Metas.Remove(meta);
            await _context.SaveChangesAsync();

            //  Redireciona para o Dashboard
            return RedirectToAction("Dashboard", "Ongs");
        }
    }
}
