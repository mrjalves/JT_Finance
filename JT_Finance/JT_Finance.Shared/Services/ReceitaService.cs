using System.Net.Http.Json;
using JT_Finance.Shared.Models;

namespace JT_Finance.Shared.Services;

public class ReceitaService : IReceitaService
{
    private readonly HttpClient _httpClient;

    public ReceitaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddReceitaAsync(ReceitaModel receita)
    {
        var response = await _httpClient.PostAsJsonAsync("api/receitas", receita);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateReceitaAsync(ReceitaModel receita)
    {
        var response = await _httpClient.PutAsJsonAsync("api/receitas", receita);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteReceitaAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/receitas/{id}");
        response.EnsureSuccessStatusCode();
    }
}
