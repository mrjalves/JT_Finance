using JT_Classes.Financeiro;

namespace JT_Finance.Shared.Services.Dashboard
{
    /// <summary>
    /// Resolve providers de indicadores por chave e expõe o catálogo disponível.
    /// </summary>
    public class DashboardIndicadorRegistry
    {
        private readonly Dictionary<string, IDashboardIndicadorProvider> _providers;

        public DashboardIndicadorRegistry(IEnumerable<IDashboardIndicadorProvider> providers)
        {
            _providers = providers.ToDictionary(p => p.Chave, p => p);
        }

        public List<DashboardIndicadorCatalogo> ObterCatalogo()
            => _providers.Values
                .Select(p => p.Catalogo)
                .OrderBy(c => c.Formato)
                .ThenBy(c => c.Titulo)
                .ToList();

        public IDashboardIndicadorProvider? ObterProvider(string chave)
            => _providers.TryGetValue(chave, out var provider) ? provider : null;
    }
}
