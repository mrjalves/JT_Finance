using JT_Finance.Shared.Models;

namespace JT_Finance.Shared.Services;

public interface IReceitaService
{
    Task AddReceitaAsync(ReceitaModel receita);
    Task UpdateReceitaAsync(ReceitaModel receita);
    Task DeleteReceitaAsync(Guid id);
}
