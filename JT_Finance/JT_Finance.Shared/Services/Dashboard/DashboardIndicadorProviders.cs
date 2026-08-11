using JT_Classes.Financeiro;
using JT_Finance.Shared.Services.Interfaces;
using JT_UserInterface.Shared.Components.Core.Charts;

namespace JT_Finance.Shared.Services.Dashboard
{
    /// <summary>
    /// Base para providers de indicadores, com utilitários comuns de formato.
    /// </summary>
    public abstract class DashboardIndicadorProviderBase : IDashboardIndicadorProvider
    {
        public abstract string Chave { get; }
        public abstract DashboardIndicadorCatalogo Catalogo { get; }

        public abstract Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia);

        protected static string FormatarMoeda(decimal valor) => valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
    }

    // ── Cards ────────────────────────────────────────────────────────────

    public class TotalReceitasProvider : DashboardIndicadorProviderBase
    {
        private readonly IReceitaService _receitaService;

        public TotalReceitasProvider(IReceitaService receitaService) => _receitaService = receitaService;

        public override string Chave => "totalReceitas";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Total Receitas (Mês)",
            Descricao = "Soma das receitas no mês de referência.",
            Formato = eDashboardFormato.Card,
            Icone = "bi-cash-coin",
            CorVariante = "success"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var total = await _receitaService.GetTotalMes(mesReferencia);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Card,
                Valor = FormatarMoeda(total),
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante
            };
        }
    }

    public class TotalDespesasProvider : DashboardIndicadorProviderBase
    {
        private readonly IDespesaService _despesaService;

        public TotalDespesasProvider(IDespesaService despesaService) => _despesaService = despesaService;

        public override string Chave => "totalDespesas";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Total Despesas (Mês)",
            Descricao = "Soma das despesas no mês de referência.",
            Formato = eDashboardFormato.Card,
            Icone = "bi-receipt",
            CorVariante = "danger"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var total = await _despesaService.GetTotalMes(mesReferencia);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Card,
                Valor = FormatarMoeda(total),
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante
            };
        }
    }

    public class SaldoProvider : DashboardIndicadorProviderBase
    {
        private readonly IReceitaService _receitaService;
        private readonly IDespesaService _despesaService;

        public SaldoProvider(IReceitaService receitaService, IDespesaService despesaService)
        {
            _receitaService = receitaService;
            _despesaService = despesaService;
        }

        public override string Chave => "saldo";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Saldo (Mês)",
            Descricao = "Receitas menos despesas no mês de referência.",
            Formato = eDashboardFormato.Card,
            Icone = "bi-piggy-bank-fill",
            CorVariante = "primary"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var receitas = await _receitaService.GetTotalMes(mesReferencia);
            var despesas = await _despesaService.GetTotalMes(mesReferencia);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Card,
                Valor = FormatarMoeda(receitas - despesas),
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante
            };
        }
    }

    public class DespesasVencidasProvider : DashboardIndicadorProviderBase
    {
        private readonly IDespesaService _despesaService;

        public DespesasVencidasProvider(IDespesaService despesaService) => _despesaService = despesaService;

        public override string Chave => "despesasVencidas";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Despesas Vencidas",
            Descricao = "Total e quantidade de despesas vencidas.",
            Formato = eDashboardFormato.Card,
            Icone = "bi-exclamation-triangle-fill",
            CorVariante = "danger"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var vencidas = await _despesaService.ObterDespesasVencidasAsync();
            var total = vencidas.Sum(d => d.Valor);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Card,
                Valor = $"{vencidas.Count} despesa(s) · {FormatarMoeda(total)}",
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante
            };
        }
    }

    public class ProximasVencimentoProvider : DashboardIndicadorProviderBase
    {
        private readonly IDespesaService _despesaService;

        public ProximasVencimentoProvider(IDespesaService despesaService) => _despesaService = despesaService;

        public override string Chave => "proximasVencimento";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Próximas ao Vencimento",
            Descricao = "Total e quantidade de despesas próximas ao vencimento.",
            Formato = eDashboardFormato.Card,
            Icone = "bi-calendar-event-fill",
            CorVariante = "warning"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var proximas = await _despesaService.ObterDespesasAVencerAsync();
            var total = proximas.Sum(d => d.Valor);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Card,
                Valor = $"{proximas.Count} despesa(s) · {FormatarMoeda(total)}",
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante
            };
        }
    }

    // ── Gráficos ─────────────────────────────────────────────────────────

    public class DespesasPorCategoriaProvider : DashboardIndicadorProviderBase
    {
        private readonly IDespesaService _despesaService;

        public DespesasPorCategoriaProvider(IDespesaService despesaService) => _despesaService = despesaService;

        public override string Chave => "despesasPorCategoria";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Despesas por Categoria",
            Descricao = "Distribuição das despesas por categoria no mês.",
            Formato = eDashboardFormato.Grafico,
            Icone = "bi-pie-chart-fill",
            CorVariante = "primary",
            TipoGrafico = "doughnut"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var dados = await _despesaService.GetDespesaPorCategoria(mesReferencia);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Grafico,
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante,
                TipoGrafico = Catalogo.TipoGrafico,
                Labels = dados.Select(d => d.Categoria).ToList(),
                Datasets = dados.Any()
                    ? new List<ChartDataset> { new() { Label = "Despesas por Categoria", Data = dados.Select(d => d.ValorTotal).ToList() } }
                    : new List<ChartDataset>(),
                SemDados = !dados.Any()
            };
        }
    }

    public class DespesasPorTipoProvider : DashboardIndicadorProviderBase
    {
        private readonly IDespesaService _despesaService;

        public DespesasPorTipoProvider(IDespesaService despesaService) => _despesaService = despesaService;

        public override string Chave => "despesasPorTipo";

        public override DashboardIndicadorCatalogo Catalogo => new()
        {
            Chave = Chave,
            Titulo = "Despesas por Tipo",
            Descricao = "Distribuição das despesas por tipo no mês.",
            Formato = eDashboardFormato.Grafico,
            Icone = "bi-bar-chart-fill",
            CorVariante = "primary",
            TipoGrafico = "bar"
        };

        public override async Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia)
        {
            var dados = await _despesaService.GetDespesaPorTipo(mesReferencia);
            return new DashboardIndicadorResultado
            {
                Titulo = Catalogo.Titulo,
                Formato = eDashboardFormato.Grafico,
                Icone = Catalogo.Icone,
                CorVariante = Catalogo.CorVariante,
                TipoGrafico = Catalogo.TipoGrafico,
                Labels = dados.Select(d => d.Tipo).ToList(),
                Datasets = dados.Any()
                    ? new List<ChartDataset> { new() { Label = "Despesas por Tipo", Data = dados.Select(d => d.ValorTotal).ToList() } }
                    : new List<ChartDataset>(),
                SemDados = !dados.Any()
            };
        }
    }
}
