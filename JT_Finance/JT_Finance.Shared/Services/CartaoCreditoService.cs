using JT_Finance.Shared.Services.Interfaces;
using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Servicos.Interfaces;

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
        private List<sCartao> _scartoes;
        public event Action OnDataChanged;
        public JT_Servicos.Financeiro.CartaoCreditoService cartaoCreditoService { get; set; }

        public CartaoCreditoService(ToastService toastService, JT_Servicos.Financeiro.CartaoCreditoService cartaoCreditoService)
        {
            _toastService = toastService;
            _scartoes = new List<sCartao>();
            this.cartaoCreditoService = cartaoCreditoService;
        }

        // Carrega (ou recarrega) os dados usando o repositório
        // NÃO dispara evento aqui para evitar loops
        public async Task CarregarDadosAsync()
        {
            // CORREÇÃO: Adicionado o 'await'
            _scartoes = await cartaoCreditoService.GetAll();
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
            // O GetById geralmente mantém-se síncrono na classe JTServiceBase
            return await cartaoCreditoService.GetById(id);
        }

        public async Task<bool> SalvarCartaoAsync(CartaoCredito cartao)
        {
            bool sucesso = await cartaoCreditoService.Save(cartao);

            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Cartão salvo com sucesso!");
                var cartaoComId = sCartao.FromEntity( await cartaoCreditoService.GetById(cartao.Id));
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
            bool sucesso = await cartaoCreditoService.Delete(id);

            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Cartão removido com sucesso!");
                // OTIMIZAÇÃO: Remove from cache instead of full reload
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
            // CORREÇÃO: Adicionado o 'await'
            return await cartaoCreditoService.ObterDespesas(id);
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
            // Removido o Task.FromResult(false) desnecessário, o compilador infere o tipo com 'async'
            if (dataPagamento == DateTime.MinValue) return false;
            if (despesas == null || !despesas.Any()) return false;

            // CORREÇÃO: Adicionado o 'await'
            bool sucesso = await cartaoCreditoService.PagarFatura(despesas, dataPagamento);

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


