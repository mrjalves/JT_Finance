using JT_Classes.Financeiro;

using JT_Finance.Shared.Repository; // Importe a nova interface
using JT_Finance.Shared.Services.Interfaces;

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
        private readonly IReceitaRep _repository; // Dependência da interface
        private List<Receita> _receitas = new();
        public event Action OnDataChanged;

        public ReceitaService(ToastService toastService, IReceitaRep repository)
        {
            _toastService = toastService;
            _repository = repository;
        }

        // Método privado para carregar ou recarregar a cache
        private async Task CarregarDadosAsync()
        {
            _receitas = await _repository.GetAllAsync();
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
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> SalvarReceitaAsync(Receita receita)
        {
            bool sucesso = await _repository.SaveAsync(receita);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita salva com sucesso!");
                // OPTIMIZATION: Use selective cache update instead of full reload
                if (receita.Id == 0 || !_receitas.Any(r => r.Id == receita.Id))
                {
                    // New item - add to cache if cached
                    if (_receitas.Any() && receita.Id > 0)
                        _receitas.Add(receita);
                }
                else
                {
                    // Existing item - update in cache
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
            bool sucesso = await _repository.DeleteAsync(id);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita removida com sucesso!");
                // OPTIMIZATION: Remove from cache instead of full reload
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

        public async Task<bool> DuplicarReceitaAsync(int id)
        {
            bool sucesso = await _repository.DuplicateAsync(id);
            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Receita duplicada com sucesso!");
                // OPTIMIZATION: Reload only on complex operation (duplicate)
                // This is acceptable since duplicate creates a new item with potentially modified fields
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
            return await _repository.GetTotalMesAsync(mes);
        }

        public async Task<List<Receita>> ObterReceitasAsync(string condicao)
        {
            return await _repository.FindAsync(condicao);
        }

        // Métodos de notificação
        public void NotificarErro(string titulo, string mensagem) => _toastService.ShowError(titulo, mensagem);
        public void NotificarSucesso(string titulo, string mensagem) => _toastService.ShowSuccess(titulo, mensagem);
        private void NotifyDataChanged() => OnDataChanged?.Invoke();
    }
}