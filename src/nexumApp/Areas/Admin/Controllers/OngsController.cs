using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OngsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public OngsController(ApplicationDbContext ctx) => _ctx = ctx;

        [HttpGet]
        public async Task<IActionResult> Pendentes()
        {
            var lista = await _ctx.Ongs
                .Where(o => o.Aprovaçao == false) 
                .ToListAsync();

            return View(lista); 
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpGet]
        public async Task<IActionResult> PendingDetails(int id)
        {
            var ong = await _ctx.Ongs
                .Include(o => o.Filials)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ong == null)
                return NotFound();

            return PartialView("_DetalhesOng", ong);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocumento(int id)
        {
            var ong = await _ctx.Ongs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ong == null)
                return NotFound("ONG não encontrada.");

            if (ong.DocumentoDados == null || ong.DocumentoDados.Length == 0)
                return NotFound("Documento não encontrado para esta ONG.");

            var contentType = string.IsNullOrWhiteSpace(ong.DocumentoTipo)
                ? "application/pdf"
                : ong.DocumentoTipo;

            var fileName = string.IsNullOrWhiteSpace(ong.DocumentoNome)
                ? $"documento-ong-{id}.pdf"
                : ong.DocumentoNome;

            return File(ong.DocumentoDados, contentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDocumento(int id)
        {
            var ong = await _ctx.Ongs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ong == null)
                return NotFound("ONG não encontrada.");

            if (ong.DocumentoDados == null || ong.DocumentoDados.Length == 0)
                return NotFound("Documento não encontrado para esta ONG.");

            var contentType = string.IsNullOrWhiteSpace(ong.DocumentoTipo)
                ? "application/pdf"
                : ong.DocumentoTipo;

            Response.Headers["Content-Disposition"] = "inline";

            return File(new MemoryStream(ong.DocumentoDados),
                        contentType,
                        fileDownloadName: null,              
                        enableRangeProcessing: true);        
        }
    }
}
