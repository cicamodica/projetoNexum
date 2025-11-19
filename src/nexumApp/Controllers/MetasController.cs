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

        public MetasController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // Pega o Id (int) da ONG com base no UserId (string) do usuário logado E BUSCA FILIAIS
        private async Task<(int? OngId, Ong Ong, ICollection<Filial> Filiais)> GetOngDataLogadaAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return (null, null, null);

            var ong = await _context.Ongs
                                    .Include(o => o.Filials) // Inclui as filiais
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(o => o.UserId == userId);

            return (ong?.Id, ong, ong?.Filials);
        }

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

        // GET: Metas/Details/5 (Não modificado)
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
        public async Task<IActionResult> Create()
        {
            // 1. Buscando as filiais para popular o modal
            var ongData = await GetOngDataLogadaAsync();
            if (ongData.OngId == null) return Forbid();

            // 2. Injeta a lista de filiais e o nome da ONG na ViewBag para uso no Razor/JavaScript
            ViewBag.Filiais = ongData.Filiais;
            ViewBag.OngNome = ongData.Ong.Nome;

            return View();
        }

        // POST: Metas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Recurso,Descricao,ValorAlvo,Status,DataFim,FilialId")] Meta meta, // FilialId já está aqui
        IFormFile imagemFile)
        {
            var ongData = await GetOngDataLogadaAsync();
            if (ongData.OngId == null) return Forbid();

            meta.OngId = ongData.OngId.Value;
            meta.ValorAtual = 0;
            meta.QuantidadeReservada = 0;

            // O FilialId é bindeado automaticamente. Se o valor for vazio, ele será null (int?).

            ModelState.Remove("Ong");      // remover a propriedade de navegação
            ModelState.Remove("ImagemUrl"); // será definida manualmente

            // VERIFICAR SE O MODELO É VÁLIDO
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
                string fullErrorMessage = "Falha na validação: " + string.Join(" | ", errorMessages);
                TempData["ErrorMessage"] = fullErrorMessage;
                return RedirectToAction("Dashboard", "Ongs");
            }

            //  TENTAR SALVAR (se o modelo for VÁLIDO)
            try
            {
                if (imagemFile != null && imagemFile.Length > 0)
                {
                    meta.ImagemUrl = await SalvarImagemAsync(imagemFile);
                }

                _context.Add(meta);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Meta criada com sucesso!";
                return RedirectToAction("Dashboard", "Ongs");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro inesperado ao salvar: " + ex.Message;
                return RedirectToAction("Dashboard", "Ongs");
            }
        }

        //função helper para salvar a imagem (Não modificado)
        private async Task<string> SalvarImagemAsync(IFormFile imagemFile)
        {
            // ... código da função (mantido o original) ...
            string nomeUnicoArquivo = Guid.NewGuid().ToString() + "_" + imagemFile.FileName;
            string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "metas");
            string caminhoArquivo = Path.Combine(pastaUploads, nomeUnicoArquivo);
            Directory.CreateDirectory(pastaUploads);
            using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await imagemFile.CopyToAsync(fileStream);
            }
            return "/uploads/metas/" + nomeUnicoArquivo;
        }

        // GET: Metas/Edit/5 (Não modificado)
        public async Task<IActionResult> Edit(int? id)
        {
            // ...
            // Se você precisar retornar a View(meta)
            // ... você também precisaria popular o ViewBag aqui.
            // ...
            return View(); // Retornando View() por simplicidade
        }


        // POST: Metas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            // ADICIONADO FilialId ao Bind
            [Bind("Id,Recurso,Descricao,ValorAlvo,Status,DataFim,FilialId")] Meta metaFormData,
            IFormFile imagemFile)
        {
            int id = metaFormData.Id;
            var metaOriginal = await _context.Metas.FindAsync(id);
            if (metaOriginal == null)
            {
                TempData["ErrorMessage"] = "Meta não encontrada.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            var ongIdLogada = await GetOngIdLogadaAsync();
            if (metaOriginal.OngId != ongIdLogada)
            {
                TempData["ErrorMessage"] = "Acesso negado.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            ModelState.Remove("imagemFile");
            ModelState.Remove("Ong");

            if (ModelState.IsValid)
            {
                try
                {
                    // Copia os dados do formulário para a meta do banco
                    metaOriginal.Recurso = metaFormData.Recurso;
                    metaOriginal.Descricao = metaFormData.Descricao;
                    metaOriginal.ValorAlvo = metaFormData.ValorAlvo;
                    metaOriginal.Status = metaFormData.Status;
                    metaOriginal.DataFim = metaFormData.DataFim;
                    metaOriginal.FilialId = metaFormData.FilialId; // <== NOVO: SALVANDO FilialId

                    // Lógica de Upload de Imagem (reaproveitando sua função)
                    if (imagemFile != null && imagemFile.Length > 0)
                    {
                        metaOriginal.ImagemUrl = await SalvarImagemAsync(imagemFile);
                    }

                    _context.Update(metaOriginal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                TempData["SuccessMessage"] = "Meta atualizada com sucesso!";
                return RedirectToAction("Dashboard", "Ongs");
            }

            var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            TempData["ErrorMessage"] = "Falha na validação: " + string.Join(" | ", errorMessages);
            return RedirectToAction("Dashboard", "Ongs");
        }

        // GET: Metas/Delete/5 (Não modificado)
        public async Task<IActionResult> Delete(int? id)
        {
            // ... (código original) ...
            return View();
        }

        // POST: Metas/Delete/5 (Não modificado)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ... (código original) ...
            return RedirectToAction("Dashboard", "Ongs");
        }

        // GET: Metas/GetMetaDetails/5
        [HttpGet]
        public async Task<IActionResult> GetMetaDetails(int id)
        {
            var ongId = await GetOngIdLogadaAsync();
            var meta = await _context.Metas
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.Id == id);

            if (meta == null || meta.OngId != ongId)
            {
                return Forbid();
            }

            // Retorna os dados necessários como JSON
            return Json(new
            {
                id = meta.Id,
                recurso = meta.Recurso,
                descricao = meta.Descricao,
                valorAlvo = meta.ValorAlvo,
                status = meta.Status,
                dataFim = meta.DataFim?.ToString("yyyy-MM-dd"),
                filialId = meta.FilialId // <== NOVO: RETORNANDO FilialId
            });
        }
    }
}
