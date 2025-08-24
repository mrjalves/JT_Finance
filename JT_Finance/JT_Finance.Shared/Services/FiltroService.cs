using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JT_Aplicacao;

using JT_Finance.Shared.Models;

namespace JT_Finance.Shared.Services
{
    public class FiltroService
    {
        public bool FiltrosVisiveis { get; private set; }
        public List<Filtro> Filtros { get; private set; } = new();
        public event Action OnChange;
        public event Func<List<Filtro>, Task> OnFiltroAplicado;
        public event Func<string, Task> OnSearchApplied;

        public void ToggleFiltros()
        {
            FiltrosVisiveis = !FiltrosVisiveis;
            NotifyStateChanged();
        }
      
        public void RegistrarFiltros(List<Filtro> filtros, Func<List<Filtro>, Task> onFiltroAplicadoCallback, Func<string, Task> onSearchAppliedCallback)
        {
            Filtros = filtros;
            OnFiltroAplicado = onFiltroAplicadoCallback;
            OnSearchApplied = onSearchAppliedCallback;
            NotifyStateChanged();
        }

        public async Task AplicarFiltros()
        {
            if (OnFiltroAplicado != null)
            {
                await OnFiltroAplicado.Invoke(Filtros);
                ToggleFiltros();
            }
        }

        public async Task AplicarPesquisa(string searchTerm)
        {
            if (OnSearchApplied != null)
            {
                await OnSearchApplied.Invoke(searchTerm);
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
