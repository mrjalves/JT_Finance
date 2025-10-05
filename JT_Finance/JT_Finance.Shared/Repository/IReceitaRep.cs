using JT_Classes.Financeiro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public interface IReceitaRep
    {
        Task<List<Receita>> GetAllAsync();
        Task<Receita> GetByIdAsync(int id);
        Task<bool> SaveAsync(Receita receita);
        Task<bool> DeleteAsync(int id);
        Task<bool> DuplicateAsync(int id);
        Task<decimal> GetTotalMesAsync(DateTime mes);
        Task<List<Receita>> FindAsync(string condicao);
    }
}
