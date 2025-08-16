using JT_Classes.Financeiro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IDespesaService
    {
        Task<List<Despesa>> ObterTodasDespesasAsync();
        Task<Despesa> ObterDespesaPorIdAsync(int id);
        Task<bool> SalvarDespesaAsync(Despesa despesa);
        Task<bool> RemoverDespesaAsync(int id);
        Task<bool> DuplicarDespesaAsync(Despesa despesa);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;

    }
}
