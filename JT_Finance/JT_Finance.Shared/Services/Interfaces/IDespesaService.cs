using JT_Finance.Shared.Services.Interfaces;
using JT_Classes.Financeiro;
using JT_Classes.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IDespesaService
    {
        Task<List<sDespesa>> ObterTodasDespesasAsync();
        Task<List<sDespesa>> ObterDespesasAsync(string condicao);
        Task<List<sDespesa>> ObterDespesasAVencerAsync();
        Task<List<sDespesa>> ObterDespesasVencidasAsync();
        Task<Despesa> ObterDespesaPorIdAsync(int id);
        Task<bool> SalvarDespesaAsync(Despesa despesa);
        Task<bool> RemoverDespesaAsync(int id);
        Task<bool> DuplicarDespesaAsync(Despesa despesa);
        Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes);
        Task<List<sDespesa>> ObterDespesasMesAsync(DateTime mes);
        Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes);
        Task<decimal> GetTotalMes(DateTime mes);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;

    }
}



