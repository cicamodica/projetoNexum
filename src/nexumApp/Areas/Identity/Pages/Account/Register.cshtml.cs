#nullable disable

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using nexumApp.Data;
using nexumApp.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace nexumApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        public RegisterModel(
            ApplicationDbContext dbContext,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            ILogger<RegisterModel> logger
           
        )
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            // USER
            [Required(ErrorMessage = "O Email é obrigatório.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A Senha é obrigatória.")]
            [StringLength(100, ErrorMessage = "A Senha deve conter entre {2} e {1} caracteres", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "A Senha e a confirmação de senha não combinam.")]
            public string ConfirmPassword { get; set; }

            //ONG
            [Required(ErrorMessage = "Obrigatório informar a Razão Social!")]
            [Display(Name = "Razão Social")]
            [StringLength(50)]
            public string Nome { get; set; }

            [Required(ErrorMessage = "Obrigatório informar a Descrição!")]
            [Display(Name = "Descrição de atividades")]
            [StringLength(300)]
            public string Descriçao { get; set; }

            [Required(ErrorMessage = "Obrigatório informar o CEP!")]
            [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve conter 8 números")]
            public string CEP { get; set; }

            [Required(ErrorMessage = "Obrigatório informar o Endereço!")]
            [StringLength(300)]
            public string Endereço { get; set; }

            [Required(ErrorMessage = "Obrigatório informar o complemento!")]
            [StringLength(100)]
            public string Complemento { get; set; }

            [Required(ErrorMessage = "Obrigatório informar o CNPJ!")]
            [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter 14 números")]
            public string CNPJ { get; set; }

            [Required(ErrorMessage = "Anexe o documento PDF para aprovação")]
            [Display(Name = "Documento PDF para aprovação")]
            public IFormFile DocumentoPdf { get; set; }

            [Display(Name = "Área de atuação")]
            [Required(ErrorMessage = "Selecione uma área de atuação")]
            public int? SelectedTagId {  get; set; }

            [Required(ErrorMessage = "Anexe uma imagem")]
            [Display(Name = "Imagem de perfil")]
            public IFormFile Image {  get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string CadastroAction, Cloudinary cloudinary)
        {
            if (ModelState.IsValid)
            {
                if (Input.DocumentoPdf == null || Input.DocumentoPdf.Length == 0)
                {
                    ModelState.AddModelError("Input.DocumentoPdf", "É obrigatório anexar um arquivo PDF.");
                    return Page();
                }
                const long maxSize = 25 * 1024 * 1024;
                if (Input.DocumentoPdf.Length > maxSize)
                {
                    ModelState.AddModelError("Input.DocumentoPdf", "Arquivo muito grande (máx 25 MB).");
                    return Page();
                }
                var isPdf = Input.DocumentoPdf.ContentType == "application/pdf" ||
                            Path.GetExtension(Input.DocumentoPdf.FileName)
                                .Equals(".pdf", StringComparison.OrdinalIgnoreCase);
                if (!isPdf)
                {
                    ModelState.AddModelError("Input.DocumentoPdf", "Apenas arquivos PDF são permitidos.");
                    return Page();
                }

                if (!Input.Image.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Input.Image", "Apenas arquivos de imagem são permitidos.");                   
                    return Page();
                }

                var cnpjEmUso = _dbContext.Ongs.Any(o => o.CNPJ == Input.CNPJ);
                if (cnpjEmUso)
                {
                    ModelState.AddModelError("Input.CNPJ", "O CNPJ informado já está em uso.");
                    return Page();
                }

                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userId = await _userManager.GetUserIdAsync(user);
                    await _userManager.AddToRoleAsync(user, "Ong");
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var ong = new Ong
                    {
                        Nome = Input.Nome,
                        Descriçao = Input.Descriçao,
                        Endereço = $"{Input.Complemento} - {Input.Endereço}",
                        CNPJ = Input.CNPJ,
                        UserId = userId,
                        Tag = Input.SelectedTagId,
                        CEP = Input.CEP
                        
                    };
                    using (var ms = new MemoryStream())
                    {
                        await Input.DocumentoPdf.CopyToAsync(ms);
                        ong.DocumentoDados = ms.ToArray();
                        ong.DocumentoTipo = "application/pdf";
                        ong.DocumentoNome = Path.GetFileName(Input.DocumentoPdf.FileName);
                    }

                    // CLOUDINARY
                    using (var ms = new MemoryStream())
                    {
                        await Input.Image.CopyToAsync(ms);
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(Input.Image.FileName, ms),
                            UseFilename = true,
                            UniqueFilename = false,
                            Overwrite = true,
                            Format = "jpg"
                        };
                        ms.Position = 0;
                        var uploadResult = cloudinary.Upload(uploadParams);
                        string imgURL = uploadResult.JsonObj.SelectToken("url")?.Value<string>();
                        ong.ImageURL = imgURL;
                    }
                    _dbContext.Add(ong);
                    await _dbContext.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    if (CadastroAction == "Cadastrar")
                    {
                        return RedirectToAction("Details", "Ongs", new { area = "", id = ong.Id });
                    }
                    return RedirectToAction("Create", "Filials", new { area = "" });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            var Tags = new Tags();
            ViewData["Tags"] = Tags.TagsList;
            return Page();
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
