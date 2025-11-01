using JT_Classes.Financeiro;
using JT_Classes.Financeiro.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IDespesaService
    {
        Task<List<sDespesaStruct>> ObterTodasDespesasAsync();
        Task<List<sDespesaStruct>> ObterDespesasAsync(string condicao);
        Task<List<sDespesaStruct>> ObterDespesasAVencerAsync();
        Task<List<sDespesaStruct>> ObterDespesasVencidasAsync();
        Task<Despesa> ObterDespesaPorIdAsync(int id);
        Task<bool> SalvarDespesaAsync(Despesa despesa);
        Task<bool> RemoverDespesaAsync(int id);
        Task<bool> DuplicarDespesaAsync(Despesa despesa);
        Task<List<TotalPorTipo>> GetDespesaPorTipo(DateTime mes);
        Task<List<sDespesaStruct>> ObterDespesasMesAsync(DateTime mes);
        Task<List<TotalPorCategoria>> GetDespesaPorCategoria(DateTime mes);
        Task<decimal> GetTotalMes(DateTime mes);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;

    }
}
