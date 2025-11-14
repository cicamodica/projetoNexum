using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using System.Threading.Tasks; 

namespace nexumApp.Controllers
{
    public class VoluntarioController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly IWebHostEnvironment _webHostEnvironment;

        
        public VoluntarioController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        
        [HttpPost]
        [Route("api/voluntario/inscrever")] 
        public async Task<IActionResult> Inscrever([FromForm] VoluntarioModel inscricao, IFormFile? imagemVoluntario)
        {
            try
            {
                
                if (imagemVoluntario != null && imagemVoluntario.Length > 0)
                {
                   
                    string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "voluntarios");

                    
                    if (!Directory.Exists(pastaUploads))
                    {
                        Directory.CreateDirectory(pastaUploads);
                    }

                  
                    string nomeArquivoUnico = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imagemVoluntario.FileName);
                    string caminhoArquivo = Path.Combine(pastaUploads, nomeArquivoUnico);

                  
                    using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
                    {
                        await imagemVoluntario.CopyToAsync(fileStream);
                    }

                    
                    inscricao.ImagemUrl = "/uploads/voluntarios/" + nomeArquivoUnico;
                }

                
                inscricao.DataAprovacao = null;

                _context.Voluntarios.Add(inscricao);

               
                await _context.SaveChangesAsync();

               
                return Json(new { success = true, message = "Inscrição realizada com sucesso!" });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, message = "Ocorreu um erro: " + ex.Message });
            }
        }
    }
}