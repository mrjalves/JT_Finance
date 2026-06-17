using JT_Finance.Shared.Services.Interfaces;
using Google.Api;

using JT_Classes.Financeiro;

using JT_Servicos.Interfaces;

using JT_UserInterface.Shared.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class ReceitaService : IReceitaService
    {
        private readonly ToastService _toastService;
        private readonly JT_Servicos.Financeiro.ReceitaService _repository; // Dependência da interface
        private List<Receita> _receitas = new();
        public event Action OnDataChanged;

        public ReceitaService(ToastService toastService, JT_Servicos.Financeiro.ReceitaService repository)
        {
            _toastService = toastService;
            _repository = repository;
        }

        // Método privado para carregar ou recarregar a cache
        private async Task CarregarDadosAsync()
        {
            // CORREÇÃO: Adicionado o 'await' porque o GetAll agora é assíncrono
            _receitas = await _repository.GetAll();
            NotifyDataChanged();
        }

        public async Task<List<Receita>> ObterTodasReceitasAsync()
        {
            if (!_receitas.Any())
            {
                await CarregarDadosAsync();
            }
            return _receitas;
        }

        public async Task<Receita> ObterReceitaPorIdAsync(int id)
        {
            // Se o GetById for síncrono na base, não usamos await. 
            // Como a função tem 'async', o C# embrulha o resultado numa Task automaticamente.
            return await _repository.GetById(id);
        }

        public async Task<bool> SalvarReceitaAsync(Receita receita)
        {
            // Save geralmente é síncrono na base
            bool sucesso = await _repository.Save(receita);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita salva com sucesso!");
                // OTIMIZAÇÃO: Atualização seletiva da cache em vez de recarga completa
                if (receita.Id == 0 || !_receitas.Any(r => r.Id == receita.Id))
                {
                    // Item novo - adiciona à cache
                    if (_receitas.Any() && receita.Id > 0)
                        _receitas.Add(receita);
                }
                else
                {
                    // Item existente - atualiza na cache
                    var existing = _receitas.FirstOrDefault(r => r.Id == receita.Id);
                    if (existing != null)
                    {
                        var index = _receitas.IndexOf(existing);
                        _receitas[index] = receita;
                    }
                }
                NotifyDataChanged();
            }
            else
            {
                NotificarErro("Erro", "Falha ao salvar receita.");
            }
            return sucesso;
        }

        public async Task<bool> RemoverReceitaAsync(int id)
        {
            // Delete geralmente é síncrono na base
            bool sucesso = await _repository.Delete(id);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita removida com sucesso!");
                // OTIMIZAÇÃO: Remove da cache em vez de recarga completa
                var toRemove = _receitas.FirstOrDefault(r => r.Id == id);
                if (toRemove != null)
                    _receitas.Remove(toRemove);
                NotifyDataChanged();
            }
            else
            {
                NotificarErro("Erro", "Falha ao remover receita.");
            }
            return sucesso;
        }

        public async Task<bool> DuplicarReceitaAsync(Receita receita)
        {
            // CORREÇÃO: Adicionado 'await' pois Duplicar agora é uma chamada à API assíncrona
            bool sucesso = await _repository.Duplicar(receita);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita duplicada com sucesso!");
                await CarregarDadosAsync();
            }
            else
            {
                NotificarErro("Erro", "Falha ao duplicar receita.");
            }
            return sucesso;
        }

        public async Task<decimal> GetTotalMes(DateTime mes)
        {
            return await _repository.GetTotalReceitaMes(mes);
        }

        public async Task<List<Receita>> ObterReceitasAsync(string condicao)
        {
            // CORREÇÃO: Adicionado 'await'
            return await _repository.Find(condicao);
        }

        public async Task<List<Receita>> ObterReceitasMesAsync(DateTime mesReferencia)
        {
            // Usamos o método GetByMonth que construímos no JT_Servicos
            return await _repository.GetByMonth(mesReferencia);
        }

        // Métodos de notificação
        public void NotificarErro(string titulo, string mensagem) => _toastService.ShowError(titulo, mensagem);
        public void NotificarSucesso(string titulo, string mensagem) => _toastService.ShowSuccess(titulo, mensagem);
        private void NotifyDataChanged() => OnDataChanged?.Invoke();

      
    }
}


