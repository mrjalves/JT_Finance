using JT_Finance.Shared.Services.Interfaces;
using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Servicos.Interfaces;

using JT_UserInterface.Shared.Services;

using System;
using System.Collections.Generic;
using System.Linq; // Necessário para o .Any()
using System.Threading.Tasks;

using static SQLite.SQLite3;
using JT_UserInterface.Shared.Services.Comum;

namespace JT_Finance.Shared.Services
{
    public class DespesaService : IDespesaService
    {
        private readonly ToastService _toastService;
        private List<sDespesa> _despesas = new();
        public event Action OnDataChanged;
        public JT_Servicos.Financeiro.DespesaService service { get; set; }

        public DespesaService(ToastService toastService, JT_Servicos.Financeiro.DespesaService repository)
        {
            _toastService = toastService;
            service = repository;
        }
         

        private async Task CarregarDadosAsync()
        {
            _despesas = await service.GetAllStruct();
        }

        public async Task<List<sDespesa>> ObterTodasDespesasAsync()
        {
            if (!_despesas.Any())
            {
                await CarregarDadosAsync();
            }
            return _despesas;
        }

        public async Task<bool> DuplicarDespesaAsync(Despesa despesa)
        {
            // Adicionado 'await' para esperar a duplicação na API
            bool sucesso = await service.Duplicar(despesa);

            if (sucesso)
            {
                _despesas.Clear();
                NotificarSucesso("Duplicada!", "A Despesa foi duplicada.");
            }
            else
                NotificarErro("Erro!", "Não foi possível duplicar a Despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<Despesa> ObterDespesaPorIdAsync(int id)
        {
            return await service.GetById(id);

        }

        public async Task<bool> RemoverDespesaAsync(int id)
        {
            bool sucesso = await service.Delete(id);
            if (sucesso)
            {
                _despesas.Clear();
                NotificarSucesso("Sucesso", "despesa removida com sucesso!");
            }
            else
                NotificarErro("Erro", "Falha ao remover despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<bool> SalvarDespesaAsync(Despesa despesa)
        {
            bool sucesso = await service.Save(despesa);
            if (sucesso)
            {
                _despesas.Clear();
                NotificarSucesso("Sucesso", "Despesa salva com sucesso!");
            }
            else
                NotificarErro("Erro", "Falha ao salvar despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<List<sDespesa>> ObterDespesasAsync(string condicao)
        {
            return await service.Find(condicao);
        }

        public async Task<List<sDespesa>> ObterDespesasMesAsync(DateTime mesReferencia)
        {
            return await service.GetDespesasPorMes(mesReferencia);
        }

        public async Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes)
        {
            return await service.GetDespesasAgrupadasPorTipo(mes);
        }

        public async Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes)
        {
            // Adicionado 'await'
            return await service.GetDespesasAgrupadasPorCategoria(mes);
        }

        public async Task<decimal> GetTotalMes(DateTime mes)
        {
            return await service.GetTotalDespesaMes(mes);
        }

        public async Task<List<sDespesa>> ObterDespesasAVencerAsync()
        {
            return await service.GetDespesasProximasAoVencimento();
        }

        public async Task<List<sDespesa>> ObterDespesasVencidasAsync()
        {
            return await service.GetDespesasVencidas();
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


