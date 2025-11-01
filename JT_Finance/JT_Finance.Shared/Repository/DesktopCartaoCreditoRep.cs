using JT_Classes.Financeiro;
using JT_Classes.Financeiro.structs;

using JT_Servicos; // Usando a camada de serviço original

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class DesktopCartaoCreditoRep : ICartaoCreditoRep
    {
     

        public Task<CartaoCredito> GetByIdAsync(int id)
        {
            return Task.FromResult(CartaoCreditoService.GetById(id));
        }

        public Task<bool> SaveAsync(CartaoCredito cartao)
        {
            return Task.FromResult(CartaoCreditoService.Save(cartao));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(CartaoCreditoService.Delete(id));
        }

        public Task<List<Despesa>> GetDespesasAsync(int cartaoId)
        {
            return Task.FromResult(CartaoCreditoService.ObterDespesas(cartaoId));
        }

        Task<List<sCartaoStruct>> ICartaoCreditoRep.GetAllAsync()
        {
            return Task.FromResult(CartaoCreditoService.GetAll());
        }
    }
}