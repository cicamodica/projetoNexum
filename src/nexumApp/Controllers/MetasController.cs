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
using System.IO; // Para upload de imagem
using Microsoft.AspNetCore.Hosting; // Para upload de imagem

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Ong")] // SÓ ONGs logadas podem acessar
    public class MetasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment; //Injetar o WebHost

        // MODIFICAR o construtor
        public MetasController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Recurso,Descricao,ValorAlvo,Status,DataFim")] Meta meta,
        IFormFile imagemFile)
        {
            var ongId = await GetOngIdLogadaAsync();
            if (ongId == null) return Forbid();

            meta.OngId = ongId.Value;
            meta.ValorAtual = 0;
            meta.QuantidadeReservada = 0;

            ModelState.Remove("Ong");      //remover a propriedade de navegação
            ModelState.Remove("ImagemUrl"); //será definida manualmente

            // VERIFICAR SE O MODELO É VÁLIDO
            if (!ModelState.IsValid)
            {
                // SE FOR INVÁLIDO:
                //Pegue todas as mensagens de erro.
                var errorMessages = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();

                // Junte-as em uma única string.
                string fullErrorMessage = "Falha na validação: " + string.Join(" | ", errorMessages);

                // Envie o erro de volta para o Dashboard via TempData.
                TempData["ErrorMessage"] = fullErrorMessage;

                // Redirecione (o que você já fazia, mas agora com uma mensagem).
                return RedirectToAction("Dashboard", "Ongs");
            }

            //  TENTAR SALVAR (se o modelo for VÁLIDO)
            try
            {
                if (imagemFile != null && imagemFile.Length > 0)
                {
                    meta.ImagemUrl = await SalvarImagemAsync(imagemFile);
                }

                _context.Add(meta);
                await _context.SaveChangesAsync();

                // SUCESSO: Envie uma mensagem de sucesso!
                TempData["SuccessMessage"] = "Meta criada com sucesso!";
                return RedirectToAction("Dashboard", "Ongs");
            }
            catch (Exception ex)
            {
                // ERRO AO SALVAR (Ex: falha de banco de dados, permissão de upload)
                // Logger.LogError(ex, "Erro ao salvar meta"); 

                // Envie a mensagem de exceção de volta.
                TempData["ErrorMessage"] = "Erro inesperado ao salvar: " + ex.Message;
                return RedirectToAction("Dashboard", "Ongs");
            }
        }

        //função helper para salvar a imagem
        private async Task<string> SalvarImagemAsync(IFormFile imagemFile)
        {
            // Gera um nome único para o arquivo
            string nomeUnicoArquivo = Guid.NewGuid().ToString() + "_" + imagemFile.FileName;

            // Define o caminho da pasta de uploads (ex: wwwroot/uploads/metas)
            string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "metas");

            // Define o caminho completo do arquivo
            string caminhoArquivo = Path.Combine(pastaUploads, nomeUnicoArquivo);

            // Garante que o diretório exista
            Directory.CreateDirectory(pastaUploads);

            // Salva o arquivo no disco
            using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await imagemFile.CopyToAsync(fileStream);
            }

            // Retorna o caminho relativo para salvar no banco
            return "/uploads/metas/" + nomeUnicoArquivo;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Recurso,Descricao,ValorAlvo,Status")] Meta meta)
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
