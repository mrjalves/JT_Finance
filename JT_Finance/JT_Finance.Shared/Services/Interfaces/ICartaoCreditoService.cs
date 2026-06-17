using JT_Finance.Shared.Services.Interfaces;
using JT_Classes.Financeiro;
using JT_Classes.structs;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface ICartaoCreditoService
    {
        Task<List<sCartao>> ObterTodosCartoesAsync();
        Task<CartaoCredito> ObterCartaoPorId(int id);
        Task<bool> SalvarCartaoAsync(CartaoCredito cartao);
        Task<bool> RemoverCartaoAsync(int id);
        Task<bool> PagarFatura(List<sCartaoDespesa> despesas, DateTime dataPagamento);
        Task<List<sCartaoDespesa>> ObterDespesas(int id);   
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        Task CarregarDadosAsync();
        event Action OnDataChanged;
    }
}


