using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JT_Classes.Financeiro;
using JT_Classes.structs;
using JT_Finance.Shared.Services.Interfaces;

namespace JT_Finance.Shared.BusinessLogic
{
    public class RelatorioIALogic
    {
        private readonly IIAAnalyticsService _iaService;
        private readonly IDespesaService _despesaService;
        private readonly IReceitaService _receitaService;
        private readonly ICartaoCreditoService _cartaoService;

        public RelatorioIALogic(
            IIAAnalyticsService iaService,
            IDespesaService despesaService,
            IReceitaService receitaService,
            ICartaoCreditoService cartaoService)
        {
            _iaService = iaService;
            _despesaService = despesaService;
            _receitaService = receitaService;
            _cartaoService = cartaoService;
        }

        public async Task<string> GerarRelatorioInteligenteAsync(DateTime? mes = null)
        {
            var mesReferencia = mes ?? DateTime.Now;

            decimal totalReceitas = 0;
            decimal totalDespesas = 0;
            List<sDespesa> despesasMes = new();
            List<sDespesa> despesasAVencer = new();
            List<sDespesa> despesasVencidas = new();
            List<TotalPorCategoria> despesasPorCategoria = new();
            List<TotalPorTipo> despesasPorTipo = new();
            List<sCartao> cartoes = new();

            try { totalReceitas = await _receitaService.GetTotalMes(mesReferencia); } catch { }
            try { totalDespesas = await _despesaService.GetTotalMes(mesReferencia); } catch { }
            try { despesasMes = await _despesaService.ObterDespesasMesAsync(mesReferencia) ?? new(); } catch { }
            try { despesasAVencer = await _despesaService.ObterDespesasAVencerAsync() ?? new(); } catch { }
            try { despesasVencidas = await _despesaService.ObterDespesasVencidasAsync() ?? new(); } catch { }
            try { despesasPorCategoria = await _despesaService.GetDespesaPorCategoria(mesReferencia) ?? new(); } catch { }
            try { despesasPorTipo = await _despesaService.GetDespesaPorTipo(mesReferencia) ?? new(); } catch { }
            try { cartoes = await _cartaoService.ObterTodosCartoesAsync() ?? new(); } catch { }

            decimal saldoLiquido = totalReceitas - totalDespesas;
            int taxaComprometimento = totalReceitas > 0 ? (int)Math.Min(100, Math.Round((totalDespesas / totalReceitas) * 100)) : (totalDespesas > 0 ? 100 : 0);

            var topCategorias = despesasPorCategoria
                .OrderByDescending(c => c.ValorTotal)
                .Take(5)
                .Select(c => new { Categoria = c.Categoria, Total = c.ValorTotal })
                .ToList();

            var resumoCartoes = cartoes
                .Select(c => new { Nome = c.Nome, Limite = c.Limite, DiaVencimento = c.DiaVencimento, Ativo = c.Ativo })
                .Take(5)
                .ToList();

            var payload = new
            {
                DataAnalise = DateTime.UtcNow,
                MesReferencia = mesReferencia.ToString("MM/yyyy"),
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                SaldoLiquido = saldoLiquido,
                TaxaComprometimento = taxaComprometimento,
                TotalDespesasMes = despesasMes.Count,
                TotalContasVencidas = despesasVencidas.Count,
                ValorTotalContasVencidas = despesasVencidas.Sum(d => d.Valor),
                TotalContasAVencer = despesasAVencer.Count,
                ValorTotalContasAVencer = despesasAVencer.Sum(d => d.Valor),
                TopCategoriasDespesa = topCategorias,
                Cartoes = resumoCartoes
            };

            string contextoJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            try
            {
                return await _iaService.GerarDiagnosticoGeralAsync(contextoJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RelatorioIALogic] Falha na IA: {ex.Message}");
                return "Diagnóstico indisponível no momento. Tente novamente em instantes.";
            }
        }

        public async Task<string> GerarSugestaoEconomiaAsync(DateTime? mes = null)
        {
            var mesReferencia = mes ?? DateTime.Now;

            List<TotalPorCategoria> despesasPorCategoria = new();
            List<TotalPorTipo> despesasPorTipo = new();
            decimal totalDespesas = 0;

            try { despesasPorCategoria = await _despesaService.GetDespesaPorCategoria(mesReferencia) ?? new(); } catch { }
            try { despesasPorTipo = await _despesaService.GetDespesaPorTipo(mesReferencia) ?? new(); } catch { }
            try { totalDespesas = await _despesaService.GetTotalMes(mesReferencia); } catch { }

            var payload = new
            {
                MesReferencia = mesReferencia.ToString("MM/yyyy"),
                TotalDespesas = totalDespesas,
                DespesasPorCategoria = despesasPorCategoria.Select(c => new { c.Categoria, c.ValorTotal }),
                DespesasPorTipo = despesasPorTipo.Select(t => new { t.Tipo, t.ValorTotal })
            };

            string contextoJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            return await _iaService.GerarSugestaoEconomiaECustosAsync(contextoJson);
        }

        public async Task<string> GerarPrevisaoFluxoAsync(DateTime? mes = null)
        {
            var mesReferencia = mes ?? DateTime.Now;

            List<sDespesa> despesasAVencer = new();
            List<sDespesa> despesasVencidas = new();
            decimal totalReceitas = 0;

            try { despesasAVencer = await _despesaService.ObterDespesasAVencerAsync() ?? new(); } catch { }
            try { despesasVencidas = await _despesaService.ObterDespesasVencidasAsync() ?? new(); } catch { }
            try { totalReceitas = await _receitaService.GetTotalMes(mesReferencia); } catch { }

            var payloadContas = new
            {
                MesReferencia = mesReferencia.ToString("MM/yyyy"),
                TotalVencidas = despesasVencidas.Count,
                ValorVencidas = despesasVencidas.Sum(d => d.Valor),
                ContasVencidas = despesasVencidas.Take(5).Select(d => new { d.Descricao, d.Valor, Data = d.DataDespesa.ToString("dd/MM/yyyy") }),
                TotalAVencer = despesasAVencer.Count,
                ValorAVencer = despesasAVencer.Sum(d => d.Valor),
                ContasAVencer = despesasAVencer.Take(5).Select(d => new { d.Descricao, d.Valor, Data = d.DataDespesa.ToString("dd/MM/yyyy") })
            };

            var payloadReceitas = new
            {
                TotalReceitasPrevistas = totalReceitas
            };

            string jsonContas = JsonSerializer.Serialize(payloadContas, new JsonSerializerOptions { WriteIndented = true });
            string jsonReceitas = JsonSerializer.Serialize(payloadReceitas, new JsonSerializerOptions { WriteIndented = true });

            return await _iaService.GerarPrevisaoFluxoECaixaAsync(jsonContas, jsonReceitas);
        }
    }
}
