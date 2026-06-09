using JT_Classes.Financeiro;
using JT_Classes.structs;
using JT_Classes.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class ApiDespRep : IDespesaRep
    {
        private readonly HttpClient _httpClient;

        public ApiDespRep(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<sDespesa>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<sDespesa>>("/api/despesas");
        }

        public async Task<Despesa> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Despesa>($"/api/despesas/{id}");
        }

        public async Task<bool> SaveAsync(Despesa despesa)
        {
            HttpResponseMessage response;
            if (despesa.Id == 0)
            {
                response = await _httpClient.PostAsJsonAsync("/api/despesas", despesa);
            }
            else
            {
                response = await _httpClient.PutAsJsonAsync($"/api/despesas/{despesa.Id}", despesa);
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/despesas/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DuplicarAsync(Despesa despesa)
        {
            var response = await _httpClient.PostAsync($"/api/despesas/{despesa.Id}/duplicar", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<sDespesa>> FindAsync(string condicao)
        {
            return await _httpClient.GetFromJsonAsync<List<sDespesa>>($"/api/despesas/find?condicao={condicao}");
        }

        public async Task<List<TotalPorTipo>> GetDespesasAgrupadasPorTipoAsync(DateTime mes)
        {
            return await _httpClient.GetFromJsonAsync<List<TotalPorTipo>>($"/api/despesas/agrupadas/tipo?mes={mes:yyyy-MM-dd}");
        }

        // --- MÉTODOS RESTANTES IMPLEMENTADOS ABAIXO ---

        public async Task<List<TotalPorCategoria>> GetDespesasAgrupadasPorCategoriaAsync(DateTime mes)
        {
            // Endpoint sugerido: GET despesas/agrupadas/categoria?mes=...
            return await _httpClient.GetFromJsonAsync<List<TotalPorCategoria>>($"/api/despesas/agrupadas/categoria?mes={mes:yyyy-MM-dd}");
        }

        public async Task<decimal> GetTotalDespesaMesAsync(DateTime mes)
        {
            return await _httpClient.GetFromJsonAsync<decimal>($"/api/despesas/total/mes?mes={mes:yyyy-MM-dd}");
        }

        public async Task<List<sDespesa>> GetDespesasProximasAoVencimentoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/despesas/proximas-vencimento");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<sDespesa>>() ?? new List<sDespesa>();
            }
            catch (Exception ex)
            {
                // Pode logar o erro ou tratar conforme a necessidade
                Console.WriteLine($"Erro ao buscar despesas próximas ao vencimento: {ex.Message}");
                return new List<sDespesa>();
            }
        }

        public async Task<List<sDespesa>> GetDespesasVencidasAsync()
        {
            // Endpoint sugerido: GET despesas/vencidas
            return await _httpClient.GetFromJsonAsync<List<sDespesa>>("/api/despesas/vencidas");
        }

        public async Task<List<sDespesa>> GetDespesasPorMesAsync(DateTime mes)
        {
            // Endpoint sugerido: GET despesas/pormes?mesReferencia=...
            return await _httpClient.GetFromJsonAsync<List<sDespesa>>($"/api/despesas/mes?mes={mes:yyyy-MM-dd}");
        }
   

      

      
        //Task<List<sDespesa>> IDespesaRep.GetDespesasProximasAoVencimentoAsync()
        //{
        //    return await _httpClient.GetFromJsonAsync<List<sDespesa>>("/api/despesas/avencer");
        //}
        //public async Task<List<Despesa>> GetDespesasProximasAoVencimentoAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync("/api/despesas/proximas-vencimento");
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadFromJsonAsync<List<Despesa>>() ?? new List<Despesa>();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Pode logar o erro ou tratar conforme a necessidade
        //        Console.WriteLine($"Erro ao buscar despesas próximas ao vencimento: {ex.Message}");
        //        return new List<Despesa>();
        //    }
        //}

    }
}
