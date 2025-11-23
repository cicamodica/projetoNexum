// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using nexumApp.Data;
using nexumApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace nexumApp.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O Email é obrigatório.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Manter conectado")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Tenta fazer o login
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                // --- CAMINHO 1: Login teve SUCESSO ---
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Pega o usuário que acabou de logar
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    // 2) Se for ADMIN → vai pro Dashboard do Admin
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }

                    // Verifica se o usuário existe e se tem a Role "Ong"
                    if (user != null && await _userManager.IsInRoleAsync(user, "Ong"))
                    {
                        // ---- Início da Lógica de Aprovação ----

                        // Encontra a entidade ONG associada a este usuário
                        var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.UserId == user.Id);

                        // Verifica se a ONG foi encontrada
                        if (ong != null)
                        {
                            if (ong.Aprovaçao == true)
                            {
                                // Adiciona new { area = "" }
                                return RedirectToAction("Dashboard", "Ongs", new { area = "" });
                            }
                            else
                            {
                                //  Adiciona new { area = "" }
                                return RedirectToAction("Details", "Ongs", new { id = ong.Id, area = "" });
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"Usuário {user.Email} tem a role 'Ong' mas não possui registro na tabela 'Ongs'.");
                            //  Adiciona new { area = "" }
                            return RedirectToAction("Wait", "Ongs", new { area = "" });
                        }
                        // ---- Fim da Lógica de Aprovação ----
                    }

                    //  Se for qualquer outro tipo de usuário (Doador, Admin, etc.)
                    //  manda para a página inicial padrão.
                    return LocalRedirect(returnUrl);
                }

                // --- CAMINHO 2: Requer 2 Fatores ---
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                // --- CAMINHO 3: Conta Bloqueada ---
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                // --- CAMINHO 4: Login FALHOU (Senha errada, etc.) ---
                else
                {
                    ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                    return Page();
                }
            }

            // --- CAMINHO 5: Modelo Inválido (Ex: email não preenchido) ---
            return Page();
        }
    }
}

