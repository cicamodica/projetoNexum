using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Filters
{
    public class VerificarPendenciasOngFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VerificarPendenciasOngFilter(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // 1. Se não estiver logado ou não for ONG, deixa passar.
            if (!user.Identity.IsAuthenticated || !user.IsInRole("Ong"))
            {
                await next();
                return;
            }

            // 2. EVITAR LOOP INFINITO:
            // Precisamos permitir que a ONG acesse as páginas onde ela corrige os dados.
            // Se a requisição já for para o OngsController e for uma ação de edição/detalhes, deixa passar.
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Lista de ações permitidas mesmo bloqueado
            var acoesPermitidas = new[] {
                "Details", "Edit", "UploadLogo", "UploadHeaderImages",
                "UpdateDescription", "UpdateSingleTag", "EditarConteudoSobre", "UpdatePix","UpdateWebsite","LogOut"
            };

            if (controller == "Ongs" && acoesPermitidas.Contains(action))
            {
                await next();
                return;
            }

            // Também permite o Logout no UserController/Identity para ele não ficar preso
            if (action == "Logout" || action == "Sair")
            {
                await next();
                return;
            }

            // 3. VERIFICAÇÃO NO BANCO
            var userId = _userManager.GetUserId(user);

            var ong = await _context.Ongs
                .AsNoTracking()
                .Select(o => new { o.Id, o.UserId, o.Aprovaçao, o.HeaderImageURL, o.ConteudoSobre, o.ChavePix })
                .FirstOrDefaultAsync(o => o.UserId == userId);

            // Lista de pendências
            var pendencias = new List<string>();

            if (ong == null)
            {
                pendencias.Add("Perfil de ONG não encontrado.");
            }
            else
            {
                if (!ong.Aprovaçao) pendencias.Add("A documentação da sua conta ainda está em avaliação.");
                if (string.IsNullOrEmpty(ong.HeaderImageURL)) pendencias.Add("Adicione uma imagem de Fundo (Capa).");
                if (string.IsNullOrEmpty(ong.ConteudoSobre)) pendencias.Add("Preencha o 'Sobre Nós' da sua ONG.");
                if (string.IsNullOrEmpty(ong.ChavePix)) pendencias.Add("Cadastre sua Chave PIX para receber doações.");
            }

            // Se houver qualquer pendência na lista
            if (pendencias.Any())
            {
                var controllerAtual = context.Controller as Controller;
                if (controllerAtual != null)
                {
                    // Passamos a lista separada por um caractere especial (ex: |) para o Front-end ler
                    controllerAtual.TempData["ListaPendencias"] = string.Join("|", pendencias);
                }

                // Redireciona para o Details (onde ele pode corrigir)
                context.Result = new RedirectToActionResult("Details", "Ongs", new { id = ong?.Id });
                return;
            }

            // Se passou por tudo, executa a página solicitada
            await next();
        }
    }
}
