using System.Collections.Generic;
using System.Threading.Tasks;
using JT_Classes.IA;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IIAAnalyticsService
    {
        Task<string> GerarDiagnosticoGeralAsync(string promptContexto);
        Task<string> GerarSugestaoEconomiaECustosAsync(string historicoDespesasJson);
        Task<string> GerarPrevisaoFluxoECaixaAsync(string contasVencimentoJson, string receitasPrevistasJson);
        Task<(bool Sucesso, string Mensagem, List<GeminiModelInfo> Modelos)> IdentificarModelosDisponiveisAsync(string apiKey);
    }
}
