using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using System.Security.Claims;
using System; 
using System.Linq; 
using System.Threading.Tasks; 

public class DoacoesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DoacoesController(ApplicationDbContext context)
    {
        _context = context;
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
        // 1. SE A VALIDAÇÃO FALHAR
        if (!ModelState.IsValid)
        {
            // Retorna um erro em JSON
            return Json(new { success = false, message = "Dados inválidos. Verifique o formulário." });
        }

        try
        {
            model.DataCriacao = DateTime.Now;
            model.Status = "Pendente";

            // SE A CONTA LOGADA É UMA ONG  Preenche automático
            if (User.IsInRole("ONG"))
            {
                int ongId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ong = await _context.Ongs.FirstOrDefaultAsync(o => o.Id == ongId);

                model.OngDoadoraId = ong.Id;
                model.OngDoadoraRazaoSocial = ong.Nome; 
            }

            _context.Doacoes.Add(model);
            await _context.SaveChangesAsync();

            // SE FOR SUCESSO
            return Json(new { success = true, message = "Doação registrada com sucesso!" });
        }
        catch (Exception ex)
        {
            // SE DER ERRO NO BANCO
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
            if (doacao == null)
            {
                return Json(new { success = false, message = "Doação não encontrada." });
            }

            var meta = await _context.Metas.FindAsync(metaId);
            if (meta == null)
            {
                return Json(new { success = false, message = "Meta não encontrada." });
            }

            string statusAnterior = doacao.Status;

            // ===== A LÓGICA PRINCIPAL DO FLUXO =====

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

            
            // Retorna tudo que a UI precisa para se atualizar
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

} 


