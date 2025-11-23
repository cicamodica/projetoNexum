using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nexumApp.Models;
using nexumApp.Models.SendEmail;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace nexumApp.Controllers
{
    public class RedefinicaoSenhaController : Controller
    {
        private readonly UserManager<User> _userManager;

        public RedefinicaoSenhaController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: página para digitar o email
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: enviar email com token
        [HttpPost]
        public async Task<IActionResult> EnviarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Mensagem"] = "Informe um e-mail válido.";
                return View("Index");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["Mensagem"] = "Este e-mail não está cadastrado.";
                return View("Index");
            }

            // gerar token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // encode para URL
            var encodedToken = WebUtility.UrlEncode(token);

            // criar link para página de redefinição
            string link = Url.Action(
                "RedefinirSenha",
                "RedefinicaoSenha",
                new { email = email, token = encodedToken },
                Request.Scheme
            );

            // enviar email
            var gmail = new Email("smtp.gmail.com", "nexum825@gmail.com", ENV.PASSWORD);

            gmail.SendEmail(
                new List<string> { email },
                "Redefinição de Senha",
                $"Clique no link para redefinir sua senha:<br><br><a href='{link}'>Redefinir Senha</a>",
                new List<string>()
            );

            TempData["Mensagem"] = "Verifique sua caixa de entrada para redefinir sua senha.";
            return View("Index");
        }

        // GET: página onde o usuário insere a nova senha
        [HttpGet]
        public IActionResult RedefinirSenha(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        // POST: salvar nova senha
        [HttpPost]
        public async Task<IActionResult> RedefinirSenha(string email, string token, string novaSenha)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Mensagem = "Usuário não encontrado.";
                return View();
            }

            // decodifica token
            var decodedToken = WebUtility.UrlDecode(token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, novaSenha);

            if (result.Succeeded)
            {
                ViewBag.Mensagem = "Sua senha foi redefinida com sucesso!";
            }
            else
            {
                // mensagens personalizadas
                var erros = "";
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordTooShort")
                        erros += "A senha deve conter no mínimo 6 caracteres.<br>";

                    else if (error.Code == "PasswordRequiresUpper")
                        erros += "A senha precisa conter pelo menos uma letra maiúscula.<br>";

                    else if (error.Code == "PasswordRequiresLower")
                        erros += "A senha precisa conter pelo menos uma letra minúscula.<br>";

                    else if (error.Code == "PasswordRequiresDigit")
                        erros += "A senha precisa conter pelo menos um número.<br>";

                    else if (error.Code == "PasswordRequiresNonAlphanumeric")
                        erros += "A senha precisa conter pelo menos um caractere especial (ex: @ ! # $ %).<br>";

                    else
                        erros += error.Description + "<br>";
                }

                ViewBag.Mensagem = erros;
            }

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }
    }
}
