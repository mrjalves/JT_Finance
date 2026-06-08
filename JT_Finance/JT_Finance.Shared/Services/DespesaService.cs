using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class DespesaService : IDespesaService
    {
        private readonly ToastService _toastService;
        private readonly IDespesaRep _repository;
        private List<sDespesa> _despesas = new();
        public event Action OnDataChanged;

        public DespesaService(ToastService toastService, IDespesaRep repository)
        {
            _toastService = toastService;
            _repository = repository;
        }

        public DespesaService()
        {
        }

        // OPTIMIZATION: Re-enable caching for improved performance
        private async Task CarregarDadosAsync()
        {
            _despesas = await _repository.GetAllAsync();
        }

        public async Task<List<sDespesa>> ObterTodasDespesasAsync()
        {
            // OPTIMIZATION: Use cache if available
            if (!_despesas.Any())
            {
                await CarregarDadosAsync();
            }
            return _despesas;
        }

        public async Task<bool> DuplicarDespesaAsync(Despesa despesa)
        {
            bool sucesso = await _repository.DuplicarAsync(despesa);
            if (sucesso)
                NotificarSucesso("Duplicada!", "A Despesa foi duplicada.");
            else
                NotificarErro("Erro!", "Não foi possível duplicar a Despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<Despesa> ObterDespesaPorIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> RemoverDespesaAsync(int id)
        {
            bool sucesso = await _repository.DeleteAsync(id);
            if (sucesso)
                NotificarSucesso("Sucesso", "despesa removida com sucesso!");
            else
                NotificarErro("Erro", "Falha ao remover despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<bool> SalvarDespesaAsync(Despesa despesa)
        {
            bool sucesso = await _repository.SaveAsync(despesa);
            if (sucesso)
                NotificarSucesso("Sucesso", "Despesa salva com sucesso!");
            else
                NotificarErro("Erro", "Falha ao salvar despesa.");

            NotifyDataChanged();
            return sucesso;
        }


        public async Task<List<sDespesa>> ObterDespesasAsync(string condicao)
        {
            return await _repository.FindAsync(condicao);
        }

        public async Task<List<sDespesa>> ObterDespesasMesAsync(DateTime mesReferencia)
        {
            return await _repository.GetDespesasPorMesAsync(mesReferencia);
        }

        public async Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes)
        {
            return await _repository.GetDespesasAgrupadasPorTipoAsync(mes);
        }

        public async Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes)
        {
            return await _repository.GetDespesasAgrupadasPorCategoriaAsync(mes);
        }

        public async Task<decimal> GetTotalMes(DateTime mes)
        {
            return await _repository.GetTotalDespesaMesAsync(mes);
        }

        public async Task<List<sDespesa>> ObterDespesasAVencerAsync()
        {
            return await _repository.GetDespesasProximasAoVencimentoAsync();
        }

        public async Task<List<sDespesa>> ObterDespesasVencidasAsync()
        {
            return await _repository.GetDespesasVencidasAsync();
        }

        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }

        private void NotifyDataChanged() => OnDataChanged?.Invoke();
    
    }
}