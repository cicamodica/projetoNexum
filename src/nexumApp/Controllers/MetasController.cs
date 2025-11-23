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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Ong")] // SÓ ONGs logadas podem acessar
    public class MetasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        private readonly Cloudinary _cloudinary;

        public MetasController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment, Cloudinary cloudinary)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _cloudinary = cloudinary;
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

        
        private async Task<string> SalvarImagemAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, ms),
                    UseFilename = true,
                    UniqueFilename = true, // Garante nomes únicos na nuvem
                    Overwrite = false,
                    
                };

                // Faz o upload para a nuvem
                var uploadResult = _cloudinary.Upload(uploadParams);

                // Retorna a URL pública da imagem hospedada
                return uploadResult.SecureUrl.ToString();
            }
        }

        // GET: Metas/Edit/5 (Não modificado)
        public async Task<IActionResult> Edit(int? id)
        {
            
            return View(); // Retornando View() por simplicidade
        }


        // POST: Metas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Recurso,Descricao,ValorAlvo,Status,DataFim,FilialId")] Meta metaFormData, IFormFile imagemFile)
        {
            if (id != metaFormData.Id) return NotFound();

            // 1. Busca a meta original no banco (com AsNoTracking para não conflitar, ou tracking normal)
            // Usamos tracking normal aqui para facilitar o update
            var metaOriginal = await _context.Metas.FindAsync(id);

            if (metaOriginal == null)
            {
                TempData["ErrorMessage"] = "Meta não encontrada.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            // 2. Segurança: Verifica se a meta pertence à ONG logada
            var ongIdLogada = await GetOngIdLogadaAsync();
            if (metaOriginal.OngId != ongIdLogada)
            {
                TempData["ErrorMessage"] = "Acesso negado.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            // 3. Limpeza do ModelState para campos que não estamos editando ou que já existem
            ModelState.Remove("Ong");
            ModelState.Remove("imagemFile"); // A imagem é opcional na edição
            ModelState.Remove("ImagemUrl");  // Já existe no banco

            if (ModelState.IsValid)
            {
                try
                {
                    // 4. Atualiza APENAS os campos permitidos com os dados do Form
                    // O formulário envia todos os campos (alterados ou não), então isso preserva o estado correto.
                    metaOriginal.Recurso = metaFormData.Recurso;
                    metaOriginal.Descricao = metaFormData.Descricao;
                    metaOriginal.ValorAlvo = metaFormData.ValorAlvo;
                    metaOriginal.Status = metaFormData.Status;
                    metaOriginal.DataFim = metaFormData.DataFim;

                    // Importante: Se FilialId for nulo (Sede), isso atualiza corretamente para null
                    metaOriginal.FilialId = metaFormData.FilialId;

                    // 5. Imagem: Só atualiza se o usuário enviou uma nova
                    if (imagemFile != null && imagemFile.Length > 0)
                    {
                        metaOriginal.ImagemUrl = await SalvarImagemAsync(imagemFile);
                    }

                    _context.Update(metaOriginal);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Meta atualizada com sucesso!";
                    return RedirectToAction("Dashboard", "Ongs");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Erro ao atualizar: " + ex.Message;
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Dados inválidos: " + string.Join(" | ", errors);
            }

            return RedirectToAction("Dashboard", "Ongs");
        }

        // POST: Metas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Obter a Meta COM os dados da ONG (Include é obrigatório aqui)
            var meta = await _context.Metas
                                     .Include(m => m.Ong) // <--- CORREÇÃO ESSENCIAL
                                     .FirstOrDefaultAsync(m => m.Id == id);

            // 2. Obter o ID do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 3. Verificações de Segurança
            if (meta == null)
            {
                TempData["ErrorMessage"] = "Meta não encontrada.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            // Verifica se a ONG dona da meta é a mesma do usuário logado
            if (meta.Ong.UserId != userId)
            {
                TempData["ErrorMessage"] = "Acesso negado: Você não pode excluir metas de outra ONG.";
                return RedirectToAction("Dashboard", "Ongs");
            }

            try
            {
                // 4. Excluir Meta
                _context.Metas.Remove(meta);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"A meta '{meta.Recurso}' foi excluída com sucesso.";
            }
            catch (Exception ex)
            {
                // Captura erros de banco (ex: se houver doações vinculadas, pode dar erro de chave estrangeira)
                TempData["ErrorMessage"] = "Não foi possível excluir. Verifique se há doações vinculadas.";
            }

            // 5. Redirecionar
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
                filialId = meta.FilialId, // Pode ser null, e o JS entende isso como "Sede"
                imagemUrl = meta.ImagemUrl // Retornamos para mostrar o preview se quiser
            });
        }
    }
}
