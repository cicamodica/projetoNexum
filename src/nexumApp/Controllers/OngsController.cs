using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
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
            var ongs = await _context.Ongs.Where(ong => ong.Aprovaçao == false).ToListAsync();
            ViewBag.Total = ongs.Count;
            return View(ongs.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Ong")] // Só ONGs logadas podem ver
        public async Task<IActionResult> Dashboard()
        {
            //  Pega o UserId (string) do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge(); // Não está logado, força login
            }

            //  Encontra a entidade ONG (pelo UserId)
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.UserId == userId);
            if (ong == null)
            {
                // Isso pode acontecer se o usuário foi deletado mas o cookie persiste
                return NotFound("Nenhuma ONG associada a este usuário.");
            }

            // Verifica a aprovação (do seu modelo Ong.cs)
            //if (ong.Aprovaçao == false)
             //{
            //return RedirectToAction(nameof(Wait)); // Redireciona se não aprovada
            //}

            //Busca as metas SOMENTE desta ONG
            var metas = await _context.Metas
                                  .Include(m => m.Ong) 
                                      .Where(m => m.OngId == ong.Id)
                   .ToListAsync();

            var vagas = await _context.Vagas
                .Where(v => v.IdONG == ong.Id)
                .ToListAsync();

            // Passamos os dados extras para a View usando o ViewBag
            ViewBag.Metas = metas;

            ViewBag.Vagas = vagas;


            // Passamos a própria ONG como o Modelo principal da View
            return View(ong);
        }

        // GET: Ongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ong = await _context.Ongs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ong == null)
            {
                return NotFound();
            }

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
    }
}
