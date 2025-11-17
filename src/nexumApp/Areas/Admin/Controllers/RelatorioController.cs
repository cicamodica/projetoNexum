using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace nexumApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "RequireAdmin")]
    public class RelatorioController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RelatorioController(ApplicationDbContext db) => _db = db;

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OngsPdf(string[] columns)
        {
            // 1) Busca metas e vagas agrupadas por ONG
            var metasPorOng = await _db.Metas
                .AsNoTracking()
                .GroupBy(m => m.OngId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            var vagasPorOng = await _db.Vagas
                .AsNoTracking()
                .GroupBy(v => v.IdONG)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            // 2) Dicionário de colunas possíveis (agora com filiais, metas e vagas)
            var allowed = new Dictionary<string, (string Header, Func<Ong, string> Value, float weight)>
            {
                ["nome"] = ("Nome Razão", o => o.Nome ?? "", 2.2f),
                ["descricao"] = ("Descrição", o => o.Descriçao ?? "", 3.0f),
                ["endereco"] = ("Endereço", o => o.Endereço ?? "", 2.4f),
                ["cnpj"] = ("CNPJ", o => FormataCnpj(o.CNPJ), 1.4f),
                ["aprovacao"] = ("Aprovação", o => o.Aprovaçao ? "Aprovada" : "Pendente", 1.2f),
                ["email"] = ("E-mail", o => o.User?.Email ?? "", 1.8f),
                ["doc"] = ("Documento", o => string.IsNullOrEmpty(o.DocumentoNome) ? "—" : o.DocumentoNome, 1.6f),
                ["filiais"] = ("Filiais", o =>
                    {
                        if (o.Filials == null || !o.Filials.Any())
                            return "Não possui filiais";

                        var detalhes = o.Filials.Select(f =>
                            $"{f.Nome} – {f.Endereço} (CNPJ {FormataCnpj(f.CNPJ)})");

                        
                        return $"{o.Filials.Count} filiais:\n- " + string.Join("\n- ", detalhes);
                    },
                    3.0f),

                ["metas"] = ("Metas", o =>
                    {
                        if (!metasPorOng.TryGetValue(o.Id, out var metas) || metas.Count == 0)
                            return "Nenhuma meta cadastrada";

                        var detalhes = metas.Select(m =>
                            $"{m.Descricao} – {m.Status} (Atual: {m.ValorAtual}, Alvo: {m.ValorAlvo})");

                        return $"{metas.Count} metas:\n- " + string.Join("\n- ", detalhes);
                    },
                    3.0f),

                ["vagas"] = ("Vagas", o =>
                    {
                        if (!vagasPorOng.TryGetValue(o.Id, out var vagas) || vagas.Count == 0)
                            return "Nenhuma vaga cadastrada";

                        var detalhes = vagas.Select(v =>
                            $"{v.Titulo} – {v.Status}");

                        return $"{vagas.Count} vagas:\n- " + string.Join("\n- ", detalhes);
                    },
                    3.0f)
            };

            //Filtra as colunas selecionadas:
            var specs = (columns ?? Array.Empty<string>())
                .Select(c => c?.Trim().ToLowerInvariant())
                .Where(c => !string.IsNullOrWhiteSpace(c) && allowed.ContainsKey(c))
                .Distinct()
                .Select(c => new { Key = c!, allowed[c!].Header, allowed[c!].Value, allowed[c!].weight })
                .ToList();

            if (specs.Count == 0)
                return BadRequest("Selecione ao menos uma coluna.");

            //Busca ONGs com User e Filials:
            var data = await _db.Ongs
                .Include(o => o.User)
                .Include(o => o.Filials)
                .AsNoTracking()
                .OrderBy(o => o.Nome)
                .ToListAsync();

            // Geração do PDF:
            var primary = Colors.Blue.Medium;
            var text = Colors.Grey.Darken4;

            byte[] pdf = Document.Create(doc =>
            {
                doc.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(25);
                    page.DefaultTextStyle(TextStyle.Default
                        .FontFamily("Poppins")
                        .FontSize(10)
                        .FontColor(text));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Relatório de ONGs")
                                .FontSize(18)
                                .SemiBold()
                                .FontColor(primary);
                            col.Item().Text(txt =>
                            {
                                txt.Span("Gerado em ").Light();
                                txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                            });
                        });
                    });

                    page.Content().Element(container =>
                    {
                        container.PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                foreach (var s in specs)
                                    cols.RelativeColumn(s.weight);
                            });

                            table.Header(header =>
                            {
                                foreach (var s in specs)
                                {
                                    header.Cell().Element(CellHeader).Text(s.Header);
                                }

                                static IContainer CellHeader(IContainer c) =>
                                    c.Background(Colors.Grey.Lighten3)
                                     .PaddingVertical(6)
                                     .PaddingHorizontal(8)
                                     .BorderBottom(1)
                                     .BorderColor(Colors.Grey.Lighten1)
                                     .DefaultTextStyle(TextStyle.Default.SemiBold());
                            });

                            var i = 0;
                            foreach (var o in data)
                            {
                                foreach (var s in specs)
                                {
                                    table.Cell().Element(e => CellBody(e, i)).Text(s.Value(o));
                                }
                                i++;
                            }

                            static IContainer CellBody(IContainer c, int rowIndex)
                            {
                                var zebra = rowIndex % 2 == 1 ? Colors.Grey.Lighten5 : Colors.White;
                                return c.Background(zebra)
                                        .PaddingVertical(5)
                                        .PaddingHorizontal(8)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten3);
                            }
                        });
                    });

                    page.Footer().AlignRight().Text(t =>
                    {
                        t.Span("Página ").Light();
                        t.CurrentPageNumber();
                        t.Span(" / ").Light();
                        t.TotalPages();
                    });
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "relatorio-ongs.pdf");
        }

        private static string FormataCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return "";
            var d = new string(cnpj.Where(char.IsDigit).ToArray());
            return d.Length == 14 ? Convert.ToUInt64(d).ToString(@"00\.000\.000\/0000\-00") : cnpj;
        }
    }
}
