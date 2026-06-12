using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.AspNetCore.Components;

using JT_Finance.Shared.Components; // Altera este namespace se o FinanceTab.razor estiver noutra pasta

namespace JT_Finance.Shared.Services
{
    /// <summary>
    /// Fornece a aba de configurações específicas do módulo Financeiro
    /// para ser injetada dinamicamente na página de Configurações Base.
    /// </summary>
    public class FinanceConfigTabProvider : IConfiguracaoTabProvider
    {
        public IEnumerable<JT_UserInterface.Shared.Models.SConfiguracaoTab> GetTabs()
        {
            // Podes usar 'yield return' para devolver várias abas se precisares no futuro.
            // Por enquanto, devolvemos apenas a Aba Financeira.
            yield return new JT_UserInterface.Shared.Models.SConfiguracaoTab
            {
                Titulo = "Financeiro",
                Conteudo = builder =>
                {
                    // Diz ao renderizador do Blazor para desenhar o nosso novo componente
                    builder.OpenComponent<FinanceTab>(0);
                    builder.CloseComponent();
                },
                Ordem = 1 // Ordem de exibição. Podes gerir isto se tiveres várias abas (ex: 2, 3...)
            };
        }
    }
}