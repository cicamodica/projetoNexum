using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers
{
    [Authorize]
    public class VagasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VagasController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdONG,Titulo,Descricao,Status")] Vaga vaga)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }
            var ong = await _context.Ongs
                                    .FirstOrDefaultAsync(o => o.UserId == userId);
            if (ong == null)
            {
                TempData["ErrorMessage"] = "Nenhuma ONG associada a este usuário.";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                vaga.IdONG = ong.Id;
                _context.Add(vaga);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Vaga criada com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erro ao criar a vaga. Verifique os dados informados.";
            }
            return RedirectToAction("Dashboard", "Ongs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vaga vaga) 
        {
            var ong = await GetOngDoUsuarioLogado();
            if (ong == null)
            {
                TempData["Error"] = "Usuário não autenticado como ONG.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            
            if (vaga.IdONG != ong.Id)
            {
                TempData["Error"] = "Você não tem permissão para editar esta vaga.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var vagaParaAtualizar = await _context.Vagas.FindAsync(vaga.IdVaga);
                    if (vagaParaAtualizar == null)
                    {
                        TempData["Error"] = "Vaga não encontrada.";
                        return RedirectToAction("Dashboard", "Ongs");
                    }

                    
                    if (vagaParaAtualizar.IdONG != ong.Id)
                    {
                        TempData["Error"] = "Operação não permitida.";
                        return RedirectToAction("Dashboard", "Ongs");
                    }

                    
                    vagaParaAtualizar.Titulo = vaga.Titulo;
                    vagaParaAtualizar.Descricao = vaga.Descricao;
                    vagaParaAtualizar.Status = vaga.Status;

                    _context.Update(vagaParaAtualizar);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Vaga atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Vagas.Any(e => e.IdVaga == vaga.IdVaga))
                    {
                        TempData["Error"] = "Vaga não encontrada.";
                    }
                    else
                    {
                        TempData["Error"] = "Erro ao atualizar a vaga.";
                    }
                }
                return RedirectToAction("Dashboard", "Ongs");
            }

            
            TempData["Error"] = "Erro ao atualizar a vaga. Verifique os dados.";
            return RedirectToAction("Dashboard", "Ongs");
        }

        [HttpGet]
        public async Task<IActionResult> GetVagaDetails(int id)
        {
            var ong = await GetOngDoUsuarioLogado();
            if (ong == null)
            {
                return Forbid(); 
            }

            var vaga = await _context.Vagas.FindAsync(id);

            if (vaga == null)
            {
                return NotFound(); 
            }

            
            if (vaga.IdONG != ong.Id)
            {
                return Forbid(); 
            }

            
            return Json(new
            {
                id = vaga.IdVaga,
                idONG = vaga.IdONG,
                titulo = vaga.Titulo,
                descricao = vaga.Descricao,
                status = vaga.Status
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaga = await _context.Vagas.FindAsync(id);
            if (vaga == null)
            {
                TempData["Error"] = "Vaga não encontrada.";
                return RedirectToAction("Dashboard", "Ongs");
            }

           
            var ong = await GetOngDoUsuarioLogado();
            if (vaga.IdONG != ong.Id)
            {
                TempData["Error"] = "Você não tem permissão para excluir esta vaga.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            _context.Vagas.Remove(vaga); 
            await _context.SaveChangesAsync();
            TempData["Success"] = "Vaga excluída com sucesso!";

            return RedirectToAction("Dashboard", "Ongs");
        }
        private async Task<Ong> GetOngDoUsuarioLogado()
        {
            var userId = _userManager.GetUserId(User);
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.UserId == userId);
            return ong;
        }
    }
}
