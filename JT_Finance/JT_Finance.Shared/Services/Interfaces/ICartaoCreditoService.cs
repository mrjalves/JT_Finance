using JT_Classes.Financeiro;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface ICartaoCreditoService
    {
        Task<List<CartaoCredito>> ObterTodosCartoesAsync();
        Task<CartaoCredito> ObterCartaoPorId(int id);
        Task<bool> SalvarCartaoAsync(CartaoCredito cartao);
        Task<bool> RemoverCartaoAsync(int id);
        Task<List<Despesa>> ObterDespesas(int id);   
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        Task CarregarDadosAsync();
        event Action OnDataChanged;
    }
}