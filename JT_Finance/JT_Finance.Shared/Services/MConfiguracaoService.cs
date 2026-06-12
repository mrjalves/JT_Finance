using JT_Aplicacao;

using JT_Finance.Shared.Models;
using JT_Finance.Shared.Services.Interfaces;

using JT_Servicos; // Importante para herdar o JTServiceBase

using System.Net.Http.Json;

namespace JT_Finance.Shared.Services
{
    /// <summary>
    /// Serviço de configurações financeiras com suporte a polimorfismo de execução.
    /// </summary>
    public class MConfiguracaoService : JTServiceBase<MConfiguracao>, IMConfiguracaoService
    {
        public MConfiguracaoService(HttpClient? httpClient = null) : base(httpClient)
        {
        }

        // Define a rota base na API para estas configurações específicas
        protected override string GetApiRoute() => "api/configuracoesfinanceiras";

        /// <summary>
        /// Vai buscar a configuração global. Se não existir, gera e guarda uma nova na base.
        /// </summary>
        public async Task<MConfiguracao> GetAtivaAsync()
        {
            // 1. Execução no Frontend (Blazor WASM) a apontar para a API
            if (IsApiMode)
            {
                if (_httpClient == null) throw new InvalidOperationException("HttpClient não configurado.");
                try
                {
                    // Assume que tens um endpoint na API que te devolve a configuração ativa
                    var config = await _httpClient.GetFromJsonAsync<MConfiguracao>($"{GetApiRoute()}/ativa");
                    if (config != null) return config;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[API Erro] Falha ao buscar MConfiguracao ativa: {ex.Message}");
                }

                // Falha de segurança para não rebentar a UI se a API estiver em baixo
                return new MConfiguracao();
            }

            // 2. Consideração do Executor: Backend a correr no Servidor (bypass de cache SQLite)
            if (Config.Instancia.Executor == "API")
            {
                return ObterOuCriarLocal();
            }

            // 3. Execução Local (Ex: Desktop ou Blazor WASM com BD local) -> Usa Cache SQLite
            string cacheKey = GetCacheKey("ConfiguracaoAtiva");
            var cached = GetFromCache<MConfiguracao>(cacheKey);
            if (cached != null) return cached;

            var localConfig = ObterOuCriarLocal();
            SaveToCache(cacheKey, localConfig);
            return localConfig;
        }

        /// <summary>
        /// Método privado para aceder ao BaseRepository e garantir a criação caso esteja vazio.
        /// </summary>
        private MConfiguracao ObterOuCriarLocal()
        {
            var config = BaseRepository.GetAll<MConfiguracao>().FirstOrDefault();

            if (config == null)
            {
                config = new MConfiguracao();
                var newId = BaseRepository.Add(config);
                config.Id = newId;
            }

            return config;
        }

        #region Overrides para forçar limpeza de Cache Personalizado

        // A classe JTServiceBase já lida bem com os Saves, mas vamos fazer override
        // apenas para atualizar a chave de cache específica "ConfiguracaoAtiva" 
        // quando houver alterações locais.

        public override async Task<bool> Save(MConfiguracao configuracao)
        {
            bool result = await base.Save(configuracao);
            if (result && Config.Instancia.Executor != "API")
            {
                SaveToCache(GetCacheKey("ConfiguracaoAtiva"), configuracao);
            }
            return result;
        }

        public override async Task<int> SaveAsync(MConfiguracao configuracao)
        {
            int resultId = await base.SaveAsync(configuracao);
            if (resultId > 0 && Config.Instancia.Executor != "API")
            {
                SaveToCache(GetCacheKey("ConfiguracaoAtiva"), configuracao);
            }
            return resultId;
        }

        #endregion
    }
}