using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Servicos; // Usando a camada de serviço original

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class DesktopCartaoCreditoRep : ICartaoCreditoRep
    {
     

        public Task<CartaoCredito> GetByIdAsync(int id)
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().GetById(id));
        }

        public Task<bool> SaveAsync(CartaoCredito cartao)
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().Save(cartao));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().Delete(id));
        }

        Task<List<sCartao>> ICartaoCreditoRep.GetAllAsync()
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().GetAll());
        }

        public Task<List<sCartaoDespesa>> ObterDespesasAsync(int cartaoId)
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().ObterDespesas(cartaoId));
        }

        public Task<bool> PagarFatura(List<sCartaoDespesa> despesas, DateTime dataPagamento)
        {
            return Task.FromResult(new JT_Servicos.CartaoCreditoService().PagarFatura(despesas, dataPagamento));
        }
    }
}