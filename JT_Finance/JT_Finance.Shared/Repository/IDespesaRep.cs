using JT_Classes.Financeiro;
using JT_Classes.Financeiro.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public interface IDespesaRep
    {
        Task<List<sDespesaStruct>> GetAllAsync();
        Task<Despesa> GetByIdAsync(int id);
        Task<bool> SaveAsync(Despesa despesa);
        Task<bool> DeleteAsync(int id);
        Task<bool> DuplicarAsync(Despesa despesa);
        Task<List<sDespesaStruct>> FindAsync(string condicao);

        // Métodos de agregação e consulta específica
        Task<List<TotalPorTipo>> GetDespesasAgrupadasPorTipoAsync(DateTime mes);
        Task<List<TotalPorCategoria>> GetDespesasAgrupadasPorCategoriaAsync(DateTime mes);
        Task<decimal> GetTotalDespesaMesAsync(DateTime mes);
        Task<List<sDespesaStruct>> GetDespesasProximasAoVencimentoAsync();
        Task<List<sDespesaStruct>> GetDespesasVencidasAsync();
        Task<List<sDespesaStruct>> GetDespesasPorMesAsync(DateTime mesReferencia);
    }
}
