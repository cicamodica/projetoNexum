using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using Microsoft.AspNetCore.Hosting; 
using System.IO;

namespace nexumApp.Controllers
{
    [Authorize]
    public class VagasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VagasController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vaga vaga, IFormFile? imagemFile)
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

                if (imagemFile != null && imagemFile.Length > 0)
                { 
                    vaga.ImagemUrl = await SalvarImagem(imagemFile);
                }

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
        public async Task<IActionResult> Edit(Vaga vaga, IFormFile? imagemFile) 
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

                    if (imagemFile != null && imagemFile.Length > 0)
                    {
                        
                        if (!string.IsNullOrEmpty(vagaParaAtualizar.ImagemUrl))
                        {
                            DeletarImagem(vagaParaAtualizar.ImagemUrl);
                        }
                       
                        vagaParaAtualizar.ImagemUrl = await SalvarImagem(imagemFile);
                    }
                    else
                    {
                        
                        vagaParaAtualizar.ImagemUrl = vaga.ImagemUrl;
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
                status = vaga.Status,
                imagemUrl = vaga.ImagemUrl,
                
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

            if (!string.IsNullOrEmpty(vaga.ImagemUrl))
            {
                DeletarImagem(vaga.ImagemUrl);
            }

            var inscricoes = _context.Inscricoes.Where(i => i.IdVaga == vaga.IdVaga);
            _context.Inscricoes.RemoveRange(inscricoes);

            
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

        private async Task<string> SalvarImagem(IFormFile imagemFile)
        {
            
            string nomeUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagemFile.FileName);

            
            string pastaImagens = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", "vagas");

        
            if (!Directory.Exists(pastaImagens))
            {
                Directory.CreateDirectory(pastaImagens);
            }

            string caminhoFisico = Path.Combine(pastaImagens, nomeUnico);

            
            await using (var fileStream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await imagemFile.CopyToAsync(fileStream);
            }

           
            return $"/imagens/vagas/{nomeUnico}";
        }

        private void DeletarImagem(string imagemUrl)
        {
            if (string.IsNullOrEmpty(imagemUrl)) return;

            
            var nomeArquivo = Path.GetFileName(imagemUrl);
            string caminhoFisico = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", "vagas", nomeArquivo);

           
            if (System.IO.File.Exists(caminhoFisico))
            {
                System.IO.File.Delete(caminhoFisico);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetFiliais()
        {
            var userId = _userManager.GetUserId(User);

            // 1. Pega a ONG do usuário logado
            var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.UserId == userId);

            if (ong == null) return Json(new List<object>());

            // 2. Busca as filiais no banco
            //  Usando "OngId" conforme sua classe Filial
            var filiais = await _context.Filials
                .Where(f => f.OngId == ong.Id)
                .Select(f => new {
                    nome = f.Nome,
                    valor = f.Endereço // O JavaScript vai usar isso para preencher o select
                })
                .ToListAsync();

            // 3. Adiciona a Sede Principal na lista
            if (!string.IsNullOrEmpty(ong.Endereço))
            {
                filiais.Insert(0, new { nome = "Sede Principal", valor = ong.Endereço });
            }

            return Json(filiais);
        }
    }
}
