using JT_Classes.Financeiro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IDespesaService
    {
        Task<List<Despesa>> ObterTodasDespesasAsync();
        Task<List<Despesa>> ObterDespesasAsync(string condicao);
        Task<List<Despesa>> ObterDespesasAVencerAsync();
        Task<List<Despesa>> ObterDespesasVencidasAsync();
        Task<Despesa> ObterDespesaPorIdAsync(int id);
        Task<bool> SalvarDespesaAsync(Despesa despesa);
        Task<bool> RemoverDespesaAsync(int id);
        Task<bool> DuplicarDespesaAsync(Despesa despesa);
        Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes);
        Task<List<Despesa>> ObterDespesasMesAsync(DateTime mes);
        Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes);
        Task<decimal> GetTotalMes(DateTime mes);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;

    }
}
