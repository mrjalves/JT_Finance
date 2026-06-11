using JT_Classes.Financeiro;
using JT_Classes.structs;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IReceitaService
    {
        Task<List<Receita>> ObterTodasReceitasAsync();
        Task<List<Receita>> ObterReceitasAsync(string condicao);
        Task<Receita> ObterReceitaPorIdAsync(int id);
        Task<bool> SalvarReceitaAsync(Receita receita);
        Task<bool> RemoverReceitaAsync(int id);
        Task<bool> DuplicarReceitaAsync(Receita receita);
        Task<decimal> GetTotalMes(DateTime mes);
        Task<List<Receita>> ObterReceitasMesAsync(DateTime mes);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;
    }
}