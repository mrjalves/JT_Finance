using JT_Classes.Financeiro;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class ApiReceitaRep : IReceitaRep
    {
        private readonly HttpClient _httpClient;

        public ApiReceitaRep(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Receita>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Receita>>("api/receitas");
        }

        public async Task<Receita> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Receita>($"api/receitas/{id}");
        }

        public async Task<bool> SaveAsync(Receita receita)
        {
            HttpResponseMessage response;
            if (receita.Id == 0)
            {
                response = await _httpClient.PostAsJsonAsync("api/receitas", receita);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"api/receitas/{receita.Id}", receita);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/receitas/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DuplicateAsync(int id)
        {
            // Endpoint de ação para duplicar um recurso
            var response = await _httpClient.PostAsync($"api/receitas/{id}/duplicar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<decimal> GetTotalMesAsync(DateTime mes)
        {
            return await _httpClient.GetFromJsonAsync<decimal>($"api/receitas/total/mes?mes={mes:yyyy-MM-dd}");
        }

        public async Task<List<Receita>> FindAsync(string condicao)
        {
            return await _httpClient.GetFromJsonAsync<List<Receita>>($"api/receitas/find?condicao={condicao}");
        }
    }
}