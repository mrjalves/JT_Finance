using JT_Classes.Financeiro;

using JT_Finance.Shared.Pages;
using JT_Finance.Shared.Services.Interfaces;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class DespesaService : IDespesaService
    {
        private readonly ToastService _toastService;
        private List<Despesa> _despesas = new();
        public event Action OnDataChanged;

        public DespesaService(ToastService toastService)
        {
            _toastService = toastService;
        }

        public async Task<List<Despesa>> ObterTodasDespesasAsync()
        {
            _despesas = new Despesa().Consultar<Despesa>("SELECT * FROM Despesas ORDER BY DataDespesa DESC");
            return await Task.FromResult(_despesas);
        }

        public async Task<bool> DuplicarDespesaAsync(Despesa despesa)
        {
            bool sucesso = despesa.Duplicar();
            if (sucesso)
                NotificarSucesso("Duplicada!", "A Despesa foi duplicada.");
            else
                NotificarErro("Erro!", "Não foi possível duplicar a Despesa.");
            NotifyDataChanged();
            return sucesso;
        }


        public async Task<Despesa> ObterDespesaPorIdAsync(int id)
        {
            await ObterTodasDespesasAsync();
            return _despesas.FirstOrDefault(r => r.Id == id);
        }

     

        public async Task<bool> RemoverDespesaAsync(int id)
        {
            var despesa = new Despesa { Id = id };
            bool sucesso = despesa.Excluir(despesa);
            await ObterTodasDespesasAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "despesa removida com sucesso!");
            else
                NotificarErro("Erro", "Falha ao remover despesa.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<bool> SalvarDespesaAsync(Despesa despesa)
        {
            bool sucesso = despesa.Salvar();
            await ObterTodasDespesasAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Despesa salva com sucesso!");
            else
                NotificarErro("Erro", "Falha ao salvar despesa.");

            NotifyDataChanged();
            return sucesso;
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

        public async Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes)
        {
            return await Task.FromResult(Despesa.GetDespesasAgrupadasPorTipo(mes));
        }

        public async  Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes)
        {
            return await Task.FromResult(Despesa.GetDespesasAgrupadasPorCategoria(mes));
        }

        public async Task<decimal> GetTotalMes(DateTime mes)
        {
            return await Task.FromResult(Despesa.GetTotalDespesaMes(mes));
        }

        public async Task<List<Despesa>> ObterDespesasAVencerAsync()
        {
            //return await Task.FromResult(Despesa.GetDespesasProximasAoVencimento());
            
            var dataInicio = DateTime.Today;
            var dataFim = new DateTime(dataInicio.Year, dataInicio.Month, 1).AddMonths(1);

            var sql = @$"
            SELECT * FROM Despesas D
            WHERE D.DataDespesa >= '{dataInicio}'AND D.DataDespesa < '{dataFim}'
            AND D.DespesaPaga = 0 AND D.EhDespesaCartaoCredito = 0
            ORDER BY D.DataDespesa DESC";
            var _ = new Despesa().Consultar<Despesa>(sql);
            sql = @$"
            SELECT * FROM Despesas D
            INNER JOIN DespesasCartaoCredito DC ON D.Id = DC.IdDespesa
            WHERE DC.ParcelaPaga = 0 AND DC.DataVencimento >= '{dataInicio}' AND DC.DataVencimento < '{dataFim}'
            ORDER BY D.DataDespesa DESC";
            _.AddRange(new Despesa().Consultar<Despesa>(sql));
            return await Task.FromResult(_);
        }

        public async Task<List<Despesa>> ObterDespesasVencidasAsync()
        {

            var dataFim = DateTime.Today;

            var sql = @$"
                SELECT * FROM Despesas D
                WHERE D.DataDespesa < '{dataFim}' AND D.EhDespesaCartaoCredito = 0
                AND D.DespesaPaga = 0 
                ORDER BY D.DataDespesa DESC";
            var _ = new Despesa().Consultar<Despesa>(sql);
            sql = @$"
                SELECT * FROM Despesas D
                INNER JOIN DespesasCartaoCredito DC ON D.Id = DC.IdDespesa
                WHERE DC.DataVencimento < '{dataFim}' AND DC.ParcelaPaga = 0 
                ORDER BY D.DataDespesa DESC";
            _.AddRange(new Despesa().Consultar<Despesa>(sql));
            return await Task.FromResult(_);

            //   return await Task.FromResult(Despesa.GetDespesasVencidas());
        }

        public async Task<List<Despesa>> ObterDespesasAsync(string condicao)
        {
            var sql = @$"SELECT * FROM Despesas {condicao}";
            return await Task.FromResult(new Despesa().Consultar<Despesa>(sql));
        }
    }
}
