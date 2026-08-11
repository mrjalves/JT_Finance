using JT_Classes.Financeiro;
using JT_UserInterface.Shared.Components.Core.Charts;

namespace JT_Finance.Shared.Services.Dashboard
{
    /// <summary>
    /// Resultado computado de um indicador para renderização (card ou gráfico).
    /// </summary>
    public class DashboardIndicadorResultado
    {
        public string Titulo { get; set; } = string.Empty;
        public eDashboardFormato Formato { get; set; } = eDashboardFormato.Card;
        public string? Valor { get; set; }
        public string Icone { get; set; } = "bi-box";
        public string CorVariante { get; set; } = "primary";
        public List<string> Labels { get; set; } = new();
        public List<ChartDataset> Datasets { get; set; } = new();
        public string TipoGrafico { get; set; } = "bar";
        public bool SemDados { get; set; }
    }

    /// <summary>
    /// Provider de um indicador do catálogo. Implementações devem ser registradas no DI.
    /// </summary>
    public interface IDashboardIndicadorProvider
    {
        string Chave { get; }
        DashboardIndicadorCatalogo Catalogo { get; }
        Task<DashboardIndicadorResultado> ResolverAsync(DateTime mesReferencia);
    }

    /// <summary>
    /// Snapshot de um indicador persistido no dashboard no momento do salvamento.
    /// Permite a visualização pública sem novas chamadas de dados.
    /// </summary>
    public class DashboardIndicadorSnapshot
    {
        public Guid IndicadorId { get; set; }
        public string ChaveIndicador { get; set; } = string.Empty;
        public DateTime MesReferencia { get; set; }
        public DashboardIndicadorResultado Resultado { get; set; } = new();
    }
}
