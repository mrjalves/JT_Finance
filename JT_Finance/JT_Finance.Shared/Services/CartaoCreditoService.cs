using JT_Classes.Financeiro;

using JT_Finance.Shared.Repository; // Importe a interface
using JT_Finance.Shared.Services.Interfaces;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class CartaoCreditoService : ICartaoCreditoService
    {
        private readonly ToastService _toastService;
        private readonly ICartaoCreditoRep _repository; // Dependência da interface
        private List<CartaoCredito> _cartoes;
        public event Action OnDataChanged;

        // Injeta a interface do repositório no construtor
        public CartaoCreditoService(ToastService toastService, ICartaoCreditoRep repository)
        {
            _toastService = toastService;
            _repository = repository;
            _cartoes = [];
        }

        // Carrega (ou recarrega) os dados usando o repositório
        public async Task CarregarDadosAsync()
        {
            _cartoes = await _repository.GetAllAsync();
            NotifyDataChanged();
        }

        public async Task<List<CartaoCredito>> ObterTodosCartoesAsync()
        {
            // Usa uma cache simples para não buscar os dados toda hora
            if (!_cartoes.Any())
            {
                await CarregarDadosAsync();
            }
            return _cartoes;
        }

        public async Task<CartaoCredito> ObterCartaoPorId(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> SalvarCartaoAsync(CartaoCredito cartao)
        {
            bool sucesso = await _repository.SaveAsync(cartao);

            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Cartão salvo com sucesso!");
                await CarregarDadosAsync(); // Recarrega a lista após salvar
            }
            else
            {
                NotificarErro("Erro", "Falha ao salvar o cartão.");
            }

            return sucesso;
        }

        public async Task<bool> RemoverCartaoAsync(int id)
        {
            bool sucesso = await _repository.DeleteAsync(id);

            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Cartão removido com sucesso!");
                await CarregarDadosAsync(); // Recarrega a lista após remover
            }
            else
            {
                NotificarErro("Erro", "Falha ao remover o cartão.");
            }

            return sucesso;
        }

        public async Task<List<Despesa>> ObterDespesas(int id)
        {
            return await _repository.GetDespesasAsync(id);
        }

        // Métodos de notificação permanecem iguais
        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }

        private void NotifyDataChanged() => OnDataChanged?.Invoke();
    }
}