using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Finance.Shared.Repository; // Importe a interface
using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared.Services;

using System;
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
        private List<sCartao> _scartoes;
        public event Action OnDataChanged;

        public CartaoCreditoService(ToastService toastService, ICartaoCreditoRep repository)
        {
            _toastService = toastService;
            _repository = repository;
            _cartoes = new List<CartaoCredito>();
            _scartoes = new List<sCartao>();
        }

        // Carrega (ou recarrega) os dados usando o repositório
        // NÃO dispara evento aqui para evitar loops
        public async Task CarregarDadosAsync()
        {
            _scartoes = await _repository.GetAllAsync();
        }

        public async Task<List<sCartao>> ObterTodosCartoesAsync()
        {
            // Use a lista correta para a cache
            if (!_scartoes.Any())
            {
                await CarregarDadosAsync();
            }
            return _scartoes;
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
                // OPTIMIZATION: Use selective cache update instead of full reload
                // This significantly reduces network calls
                var cartaoComId = await _repository.GetByIdAsync(cartao.Id);
                if (cartaoComId != null)
                {
                    var existing = _scartoes.FirstOrDefault(c => c.Id == cartao.Id);
                    if (existing != null)
                    {
                        var index = _scartoes.IndexOf(existing);
                        _scartoes[index] = new sCartao { Id = cartaoComId.Id, Nome = cartaoComId.Nome, Limite = cartaoComId.Limite };
                    }
                    else if (_scartoes.Any())
                    {
                        _scartoes.Add(new sCartao { Id = cartaoComId.Id, Nome = cartaoComId.Nome, Limite = cartaoComId.Limite });
                    }
                }
                NotifyDataChanged();
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
                // OPTIMIZATION: Remove from cache instead of full reload
                var toRemove = _scartoes.FirstOrDefault(c => c.Id == id);
                if (toRemove != null)
                    _scartoes.Remove(toRemove);
                NotifyDataChanged();
            }
            else
            {
                NotificarErro("Erro", "Falha ao remover o cartão.");
            }

            return sucesso;
        }

        public async Task<List<sCartaoDespesa>> ObterDespesas(int id)
        {
            return await _repository.ObterDespesasAsync(id);
        }

        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }

        private void NotifyDataChanged() => OnDataChanged?.Invoke();

        public async Task<bool> PagarFatura(List<sCartaoDespesa> despesas, DateTime dataPagamento)
        {
            if (dataPagamento == DateTime.MinValue)return await Task.FromResult(false);
            if (despesas == null || !despesas.Any()) return await Task.FromResult(false);
            bool sucesso = _repository.PagarFatura(despesas, dataPagamento).Result;
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Fatura paga com sucesso!");
                NotifyDataChanged();

            }
            else
            {
                NotificarErro("Erro", "Falha ao pagar a fatura."); 
                NotifyDataChanged();
            }
            return sucesso;
        }
    }
}