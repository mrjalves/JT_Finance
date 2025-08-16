using JT_Classes.Financeiro;

using JT_Finance.Shared.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class ReceitaService : IReceitaService
    {
        private readonly ToastService _toastService;
        private List<Receita> _receitas = new();
        public event Action OnDataChanged;

        public ReceitaService(ToastService toastService)
        {
            _toastService = toastService;
        }

        public async Task<List<Receita>> ObterTodasReceitasAsync()
        {
            _receitas = new Receita().Consultar<Receita>("SELECT * FROM Receitas ORDER BY DataReceita DESC");
            return await Task.FromResult(_receitas);
        }

        public async Task<Receita> ObterReceitaPorIdAsync(int id)
        {
            await ObterTodasReceitasAsync();
            return _receitas.FirstOrDefault(r => r.Id == id);
        }

        public async Task<bool> SalvarReceitaAsync(Receita receita)
        {
            bool sucesso = receita.Salvar();
            await ObterTodasReceitasAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Receita salva com sucesso!");
            else
                NotificarErro("Erro", "Falha ao salvar receita.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<bool> RemoverReceitaAsync(int id)
        {
            var receita = new Receita { Id = id };
            bool sucesso = receita.Excluir(receita);
            await ObterTodasReceitasAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Receita removida com sucesso!");
            else
                NotificarErro("Erro", "Falha ao remover receita.");

            NotifyDataChanged();
            return sucesso;
        }

        public async Task<bool> DuplicarReceitaAsync(int id)
        {
            var receita = await ObterReceitaPorIdAsync(id);
            if (receita == null) return false;

            bool sucesso = receita.Duplicar();
            await ObterTodasReceitasAsync();

            if (sucesso)
                NotificarSucesso("Sucesso", "Receita duplicada com sucesso!");
            else
                NotificarErro("Erro", "Falha ao duplicar receita.");

            NotifyDataChanged();
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

        private void NotifyDataChanged() => OnDataChanged?.Invoke();
    }
}