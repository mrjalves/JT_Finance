using JT_Classes.Financeiro;
using JT_Classes.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public interface IDespesaRep
    {
        Task<List<sDespesa>> GetAllAsync();
        Task<Despesa> GetByIdAsync(int id);
        Task<bool> SaveAsync(Despesa despesa);
        Task<bool> DeleteAsync(int id);
        Task<bool> DuplicarAsync(Despesa despesa);
        Task<List<sDespesa>> FindAsync(string condicao);

        // Métodos de agregação e consulta específica
        Task<List<TotalPorTipo>> GetDespesasAgrupadasPorTipoAsync(DateTime mes);
        Task<List<TotalPorCategoria>> GetDespesasAgrupadasPorCategoriaAsync(DateTime mes);
        Task<decimal> GetTotalDespesaMesAsync(DateTime mes);
        Task<List<sDespesa>> GetDespesasProximasAoVencimentoAsync();
        Task<List<sDespesa>> GetDespesasVencidasAsync();
        Task<List<sDespesa>> GetDespesasPorMesAsync(DateTime mesReferencia);
    }
}
