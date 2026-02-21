using JT_Classes.Financeiro;
using JT_Classes.structs;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public interface ICartaoCreditoRep
    {
        Task<List<sCartao>> GetAllAsync();
        Task<CartaoCredito> GetByIdAsync(int id);
        Task<bool> SaveAsync(CartaoCredito cartao);
        Task<bool> DeleteAsync(int id);
        Task<List<Despesa>> GetDespesasAsync(int cartaoId);
    }
}