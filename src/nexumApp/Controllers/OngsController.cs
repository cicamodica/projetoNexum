using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using nexumApp.Areas.Identity.Pages.Account;
using nexumApp.Data;
using nexumApp.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace nexumApp.Controllers
{
    public class OngsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OngsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        // GET: Ongs
        public async Task<IActionResult> Index()
        {
            var ongs = await _context.Ongs.Where(ong => ong.Aprovaçao == true).ToListAsync();
            return View(ongs);
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
            if (ong.Aprovaçao == false)
            {
                return RedirectToAction(nameof(Wait)); // Redireciona se não aprovada
            }

            //Busca as metas SOMENTE desta ONG
            var metas = await _context.Metas
                                      .Where(m => m.OngId == ong.Id)
                                      .ToListAsync();

           
            // Passamos os dados extras para a View usando o ViewBag
            ViewBag.Metas = metas;
            

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

        public IActionResult Create()
        {
            string Email = TempData.Peek("Email")?.ToString();
            string Password = TempData.Peek("Password")?.ToString();
            if (Email == null || Password == null)
            {
                return NotFound();
            }
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

                string Email = TempData["Email"].ToString();
                string Password = TempData["Password"].ToString();

                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.AddToRoleAsync(user, "Ong");
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    ong.UserId = userId;
                    _context.Add(ong);
                    await _context.SaveChangesAsync();
                    TempData.Remove("Email");
                    TempData.Remove("Password");
                    return RedirectToAction(nameof(Wait));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
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
  
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
  
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
