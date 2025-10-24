using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using System.Security.Claims;

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Ong")]
    public class OngsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OngsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ongs
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var ongs = await _context.Ongs.Where(ong => ong.Aprovaçao == true).ToListAsync();
            return View(ongs);
        }

        // GET: Ongs/Details/5
        [AllowAnonymous]
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

        // GET: Ongs/Create
        [Authorize(Policy = "HasCreatedOrApprovedONG")]
        public async Task<IActionResult> CreateAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ongs = await _context.Ongs.Where(ong => ong.UserId == userId).ToListAsync();
            var isFirstOng = ongs.Count() == 0;
            ViewBag.IsFirstOng = isFirstOng;
            return View();
        }

        // POST: Ongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descriçao,Endereço,CNPJ,DocumentoPdf")] Ong ong)
        {
            if (ModelState.IsValid)
            {
                if (ong.DocumentoPdf == null || ong.DocumentoPdf.Length == 0)
                {
                    ModelState.AddModelError(nameof(ong.DocumentoPdf), "É obrigatório anexar um arquivo PDF.");
                    return View(ong);
                }

                const long maxSize = 25 * 1024 * 1024; 
                if (ong.DocumentoPdf.Length > maxSize)
                {
                    ModelState.AddModelError(nameof(ong.DocumentoPdf), "Arquivo muito grande (máx 25 MB).");
                    return View(ong);
                }

                var isPdf = ong.DocumentoPdf.ContentType == "application/pdf" ||
                            Path.GetExtension(ong.DocumentoPdf.FileName)
                                .Equals(".pdf", StringComparison.OrdinalIgnoreCase);
                if (!isPdf)
                {
                    ModelState.AddModelError(nameof(ong.DocumentoPdf), "Apenas arquivos PDF são permitidos.");
                    return View(ong);
                }

                using (var ms = new MemoryStream())
                {
                    await ong.DocumentoPdf.CopyToAsync(ms);
                    ong.DocumentoDados = ms.ToArray();
                    ong.DocumentoTipo = "application/pdf";
                    ong.DocumentoNome = Path.GetFileName(ong.DocumentoPdf.FileName);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ong.UserId = userId;
                _context.Add(ong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Wait));
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
        public async Task<IActionResult> Wait()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var total = await _context.Ongs.CountAsync(o => o.UserId == userId);

            ViewBag.Message = (total <= 1)
                ? "Aguardando aprovação de cadastro da ONG"
                : "Aguardando aprovação de filial da ONG";

            return View();
        }



        private bool OngExists(int id)
        {
            return _context.Ongs.Any(e => e.Id == id);
        }
    }
}
