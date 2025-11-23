using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nexumApp.Data;
using nexumApp.Models;
using nexumApp.Services; // Importante para reconhecer o serviço definido lá embaixo
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Mail;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Adiciona o filtro globalmente
    options.Filters.Add<nexumApp.Filters.VerificarPendenciasOngFilter>();
});

// 1. Configuração do Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. REGISTRO DO SERVIÇO DE E-MAIL 
// Ensina o sistema que quando pedir IEmailService, deve entregar EmailService
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Configuração de Identidade
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<DuplicateUserDescriber>();

builder.Services.AddControllersWithViews();

// 4. Políticas de Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasCreatedOrApprovedONG", policy =>
    {
        policy.Requirements.Add(new HasCreatedOrApprovedONGRequirement());
    });

    options.AddPolicy("RequireAdmin", p =>
       p.RequireAuthenticatedUser()
        .RequireRole("Admin"));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasCreatedOrApprovedONGRequirementHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configuração do Cloudinary

DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
string cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
if (!string.IsNullOrEmpty(cloudinaryUrl))
{
    Cloudinary cloudinary = new Cloudinary(cloudinaryUrl);
    cloudinary.Api.Secure = true;

    builder.Services.AddSingleton(cloudinary);
}

// 5. Método Seeder (Função Local)
static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
{
    using var scope = services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var email = config["AdminBootstrap:Email"];
    var pwd = config["AdminBootstrap:Password"];

    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(pwd))
    {
        var user = await userMgr.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User { UserName = email, Email = email, EmailConfirmed = true };
            var create = await userMgr.CreateAsync(user, pwd);
            if (!create.Succeeded)
                throw new Exception(string.Join("; ", create.Errors.Select(e => e.Description)));
        }
        if (!await userMgr.IsInRoleAsync(user, "Admin"))
            await userMgr.AddToRoleAsync(user, "Admin");
    }
}

var app = builder.Build();

// 6. Execução do Seeder de Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Ong" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

await SeedAdminAsync(app.Services, builder.Configuration);

// 7. Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}





app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();


namespace nexumApp.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailDestino, string assunto, string mensagemHtml);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailDestino, string assunto, string mensagemHtml)
        {
            try
            {
                var mailUser = _configuration["EmailSettings:Mail"];
                var mailName = _configuration["EmailSettings:DisplayName"];
                var mailPassword = _configuration["EmailSettings:Password"];
                var mailHost = _configuration["EmailSettings:Host"];
                // Validação simples para evitar erro se a porta não estiver configurada
                int mailPort = 587;
                if (int.TryParse(_configuration["EmailSettings:Port"], out int parsedPort))
                {
                    mailPort = parsedPort;
                }

                using (var client = new SmtpClient(mailHost, mailPort))
                {
                    client.Credentials = new NetworkCredential(mailUser, mailPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(mailUser, mailName),
                        Subject = assunto,
                        Body = mensagemHtml,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(emailDestino);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Em produção, o ideal é logar o erro em vez de jogar na tela
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                throw; // Repassa o erro para ser tratado pelo Controller se necessário
            }
        }
    }
}
