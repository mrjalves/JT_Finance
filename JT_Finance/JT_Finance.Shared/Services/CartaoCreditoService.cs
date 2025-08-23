using JT_Classes.Financeiro;

using JT_Finance.Shared.Services.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class CartaoCreditoService : ICartaoCreditoService
    {
        private readonly ToastService _toastService;
        private List<CartaoCredito> _cartoes;
        public event Action OnDataChanged;

        public CartaoCreditoService(ToastService toastService)
        {
            _toastService = toastService;
            _cartoes = [];
        }

        public async Task CarregarDadosAsync()
        {
            _cartoes = JT_Servicos.CartaoCreditoService.GetAll();
            await Task.CompletedTask;
            NotifyDataChanged();
        }

        public async Task<List<CartaoCredito>> ObterTodosCartoesAsync()
        {
            if (!_cartoes.Any())
                await CarregarDadosAsync();

            return _cartoes;
        }

        public async Task<CartaoCredito> ObterCartaoPorId(int id)
        {
            return await Task.FromResult(JT_Servicos.CartaoCreditoService.GetById(id));
        }

        public async Task<bool> SalvarCartaoAsync(CartaoCredito cartao)
        {
            bool sucesso = JT_Servicos.CartaoCreditoService.Save(cartao);
            await CarregarDadosAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Cartão salvo com sucesso!");
            else
                NotificarErro("Erro", "Falha ao salvar o cartão.");

            return sucesso;
        }

        public async Task<bool> RemoverCartaoAsync(int id)
        {
            bool sucesso = JT_Servicos.CartaoCreditoService.Delete(id);
            await CarregarDadosAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Cartão removido com sucesso!");
            else
                NotificarErro("Erro", "Falha ao remover o cartão.");

            return sucesso;
        }

        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }
        // Mantenha a implementação existente que já contém o evento
        private void NotifyDataChanged() => OnDataChanged?.Invoke();

        public async Task<List<Despesa>> ObterDespesas(int id)
        {
            return await Task.FromResult(JT_Servicos.CartaoCreditoService.ObterDespesas(id));
        }
    }
}