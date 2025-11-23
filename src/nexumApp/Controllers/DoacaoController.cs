using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using nexumApp.Services;
using System; 
using System.Linq; 
using System.Security.Claims;
using System.Threading.Tasks;

public class DoacoesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService; // 1. Adicionamos o serviço aqui

    // 2. Atualizamos o construtor para receber o IEmailService
    public DoacoesController(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // LISTA DE DOAÇÕES
    public async Task<IActionResult> Index()
    {
        ViewBag.Ongs = await _context.Ongs.ToListAsync();
        var doacoes = await _context.Doacoes
            .Include(d => d.Meta.Ong)
            .OrderByDescending(d => d.DataCriacao)
            .ToListAsync();

        return View(doacoes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Doacao model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Dados inválidos. Verifique o formulário." });
        }

        try
        {
            model.DataCriacao = DateTime.Now;
            model.Status = "Pendente";

            if (User.IsInRole("ONG"))
            {
                int ongId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId);
                model.OngDoadoraId = ong.Id;
                model.OngDoadoraRazaoSocial = ong.Nome;
            }

            // --- BUSCA DADOS DA META E DA ONG DONA DA META (Para o e-mail) ---
            var metaInfo = await _context.Metas
                .Include(m => m.Ong)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == model.MetaId);

            if (metaInfo == null)
            {
                return Json(new { success = false, message = "A meta selecionada não existe." });
            }

            // Salva a doação
            _context.Doacoes.Add(model);
            await _context.SaveChangesAsync();

            // --- ENVIA O E-MAIL USANDO OS NOMES CORRETOS DO SEU MODEL ---
            // Aqui corrigimos: model.EmailDoador -> model.Email
            // model.NomeDoador -> model.NomeCompleto
            // model.TelefoneDoador -> model.Telefone

            await EnviarEmailConfirmacao(
                model.Email,          // <--- Corrigido (era EmailDoador)
                model.NomeCompleto,   // <--- Corrigido (era NomeDoador)
                metaInfo.Ong.Nome,
                metaInfo.Recurso,
                model.Telefone        // <--- Corrigido (era TelefoneDoador)
            );

            return Json(new { success = true, message = "Doação registrada com sucesso! Verifique seu e-mail." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Ocorreu um erro ao salvar no banco de dados." });
        }
    }

    // GET: /Doacoes/GetDoacoesParaMeta?metaId=5
    [HttpGet]
    public async Task<IActionResult> GetDoacoesParaMeta(int metaId)
    {
        var doacoes = await _context.Doacoes
                                    .Where(d => d.MetaId == metaId)
                                    .OrderByDescending(d => d.DataCriacao)
                                    .ToListAsync();

        return PartialView("~/Views/Home/_DoacoesListaPartial.cshtml", doacoes);
    }

    // POST: /Doacoes/AtualizarStatusDoacao
    [HttpPost]
    public async Task<IActionResult> AtualizarStatusDoacao(int doacaoId, string status, int metaId)
    {
        try
        {
            var doacao = await _context.Doacoes.FindAsync(doacaoId);
            if (doacao == null) return Json(new { success = false, message = "Doação não encontrada." });

            var meta = await _context.Metas.FindAsync(metaId);
            if (meta == null) return Json(new { success = false, message = "Meta não encontrada." });

            string statusAnterior = doacao.Status;

            // LÓGICA DE ATUALIZAÇÃO DE SALDO
            if (status == "Confirmada" && statusAnterior != "Confirmada")
            {
                meta.ValorAtual += doacao.Quantidade;
            }
            else if (status != "Confirmada" && statusAnterior == "Confirmada")
            {
                meta.ValorAtual -= doacao.Quantidade;
            }

            doacao.Status = status;

            _context.Update(doacao);
            _context.Update(meta);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                novoValorAtual = meta.ValorAtual,
                valorAlvo = meta.ValorAlvo
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Um erro ocorreu ao salvar no banco." });
        }
    }

    // =========================================================================
    // MÉTODO AUXILIAR PRIVADO PARA ENVIO DE E-MAIL
    // =========================================================================
    private async Task EnviarEmailConfirmacao(string emailDoador, string nomeDoador, string nomeOng, string nomeMeta, string telefoneDoador)
    {
        string assunto = $"Confirmação de Interesse de Doação - {nomeOng}";

        string corpoEmail = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #16435D; padding: 20px; text-align: center;'>
                    <h2 style='color: #ffffff; margin: 0;'>Obrigado, {nomeDoador}!</h2>
                </div>
                <div style='padding: 30px; color: #333;'>
                    <p style='font-size: 16px;'>Olá!</p>
                    <p>Recebemos sua intenção de doar para a meta <strong>{nomeMeta}</strong> da ONG <strong>{nomeOng}</strong>.</p>
                    
                    <div style='background-color: #f0f8ff; border-left: 5px solid #16435D; padding: 15px; margin: 20px 0;'>
                        <h3 style='margin-top: 0; color: #16435D;'>O que acontece agora?</h3>
                        <p>A ONG já foi notificada.</p>
                        <p>A equipe entrará em contato com você pelo telefone <strong>{telefoneDoador}</strong> ou por este e-mail para combinar a entrega ou retirada dos itens.</p>
                    </div>

                    <p>Agradecemos imensamente sua solidariedade!</p>
                </div>
                <div style='background-color: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #888;'>
                    <p>Equipe Nexum</p>
                </div>
            </div>";

        // Dispara o email
        if (!string.IsNullOrEmpty(emailDoador))
        {
            await _emailService.SendEmailAsync(emailDoador, assunto, corpoEmail);
        }
    }
}


