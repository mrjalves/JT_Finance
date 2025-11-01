using JT_Classes.Financeiro;
using JT_Classes.Financeiro.structs;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class ApiCartaoCreditoRep : ICartaoCreditoRep
    {
        private readonly HttpClient _httpClient;

        public ApiCartaoCreditoRep(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CartaoCredito>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CartaoCredito>>("api/cartoescredito");
        }

        public async Task<CartaoCredito> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CartaoCredito>($"api/cartoescredito/{id}");
        }

        public async Task<bool> SaveAsync(CartaoCredito cartao)
        {
            HttpResponseMessage response;
            if (cartao.Id == 0)
            {
                // Criar novo cartão
                response = await _httpClient.PostAsJsonAsync("api/cartoescredito", cartao);
            }
            else
            {
                // Atualizar cartão existente
                response = await _httpClient.PutAsJsonAsync($"api/cartoescredito/{cartao.Id}", cartao);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/cartoescredito/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Despesa>> GetDespesasAsync(int cartaoId)
        {
            // Endpoint para buscar um recurso aninhado (despesas de um cartão)
            return await _httpClient.GetFromJsonAsync<List<Despesa>>($"api/cartoescredito/{cartaoId}/despesas");
        }

        Task<List<sCartaoStruct>> ICartaoCreditoRep.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}