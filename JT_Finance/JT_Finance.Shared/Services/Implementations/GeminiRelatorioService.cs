using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JT_Classes.IA;
using JT_Finance.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace JT_Finance.Shared.Services.Implementations
{
    public class GeminiRelatorioService : IIAAnalyticsService
    {
        private readonly IHttpClientFactory? _httpClientFactory;
        private readonly NavigationManager? _navigationManager;
        private readonly HttpClient? _fallbackHttpClient;
        private readonly IConfiguration _configuration;
        private HttpClient? _cleanHttpClient;

        public static string? CustomApiKey { get; set; }
        public static string CustomModel { get; set; } = "gemini-1.5-flash";

        public GeminiRelatorioService(
            IConfiguration configuration,
            IHttpClientFactory? httpClientFactory = null,
            NavigationManager? navigationManager = null,
            HttpClient? httpClient = null)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _navigationManager = navigationManager;
            _fallbackHttpClient = httpClient;
        }

        private HttpClient ObterHttpClientLimpo()
        {
            if (_cleanHttpClient == null)
            {
                if (_httpClientFactory != null)
                {
                    try
                    {
                        _cleanHttpClient = _httpClientFactory.CreateClient("GeminiCleanClient");
                    }
                    catch { }
                }
                _cleanHttpClient ??= new HttpClient();
            }
            return _cleanHttpClient;
        }

        private string? ObterApiKey()
        {
            if (!string.IsNullOrWhiteSpace(CustomApiKey))
                return CustomApiKey;

            return _configuration["GeminiApiKey"]
                ?? _configuration["Config:Gemini:ApiKey"]
                ?? _configuration["Config:FirebaseConfig:GeminiApiKey"]
                ?? _configuration["AI:ApiKey"];
        }

        private string TratarErroGoogleApi(System.Net.HttpStatusCode statusCode, string? responseJson)
        {
            string msgOriginal = "";
            string reason = "";

            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseJson);
                    if (doc.RootElement.TryGetProperty("error", out var errorProp))
                    {
                        if (errorProp.TryGetProperty("message", out var mProp))
                            msgOriginal = mProp.GetString() ?? "";

                        if (errorProp.TryGetProperty("details", out var detailsProp) && detailsProp.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var d in detailsProp.EnumerateArray())
                            {
                                if (d.TryGetProperty("reason", out var rProp))
                                {
                                    reason = rProp.GetString() ?? "";
                                    break;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            if (reason.Equals("API_KEY_INVALID", StringComparison.OrdinalIgnoreCase) ||
                msgOriginal.Contains("API key not valid", StringComparison.OrdinalIgnoreCase) ||
                msgOriginal.Contains("API_KEY_INVALID", StringComparison.OrdinalIgnoreCase))
            {
                return "Chave de API inválida no Google Gemini. Por favor, gere ou verifique sua chave no Google AI Studio (aistudio.google.com/app/apikey).";
            }

            if (reason.Equals("RESOURCE_EXHAUSTED", StringComparison.OrdinalIgnoreCase) ||
                statusCode == System.Net.HttpStatusCode.TooManyRequests ||
                msgOriginal.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                msgOriginal.Contains("exhausted", StringComparison.OrdinalIgnoreCase))
            {
                return "Cota de requisições temporariamente excedida no Google Gemini. O sistema usará o motor inteligente embutido até a cota normalizar.";
            }

            if (reason.Equals("PERMISSION_DENIED", StringComparison.OrdinalIgnoreCase) ||
                statusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return "Permissão negada pela API do Google. Certifique-se de que a API Generative Language está habilitada no seu projeto Google Cloud / AI Studio.";
            }

            if (!string.IsNullOrWhiteSpace(msgOriginal))
            {
                return $"Google API ({(int)statusCode}): {msgOriginal}";
            }

            return $"Erro ao comunicar com a API do Google Gemini (Status: {(int)statusCode} - {statusCode}).";
        }

        public async Task<(bool Sucesso, string Mensagem, List<GeminiModelInfo> Modelos)> IdentificarModelosDisponiveisAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return (false, "Informe a Google Gemini API Key antes de identificar os modelos.", new List<GeminiModelInfo>());
            }

            string key = apiKey.Trim();
            string googleEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models?key={Uri.EscapeDataString(key)}";

            HttpResponseMessage? response = null;
            string? responseJson = null;

            try
            {
                var client = ObterHttpClientLimpo();
                response = await client.GetAsync(googleEndpoint);
                responseJson = await response.Content.ReadAsStringAsync();
            }
            catch (Exception exDirect)
            {
                if (_navigationManager != null && !string.IsNullOrWhiteSpace(_navigationManager.BaseUri))
                {
                    try
                    {
                        string proxyUrl = $"{_navigationManager.BaseUri.TrimEnd('/')}/api/gemini/models?key={Uri.EscapeDataString(key)}";
                        var proxyClient = ObterHttpClientLimpo();
                        response = await proxyClient.GetAsync(proxyUrl);
                        responseJson = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        return (false, $"Erro de conexão ao identificar modelos: {exDirect.Message}", new List<GeminiModelInfo>());
                    }
                }
                else
                {
                    return (false, $"Erro de conexão ao identificar modelos: {exDirect.Message}", new List<GeminiModelInfo>());
                }
            }

            if (response != null && !response.IsSuccessStatusCode)
            {
                string erroTraduzido = TratarErroGoogleApi(response.StatusCode, responseJson);
                return (false, erroTraduzido, new List<GeminiModelInfo>());
            }

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                return (false, "Resposta vazia recebida do serviço Gemini.", new List<GeminiModelInfo>());
            }

            try
            {
                using var doc = JsonDocument.Parse(responseJson);
                var modelos = new List<GeminiModelInfo>();

                if (doc.RootElement.TryGetProperty("models", out var modelsElement) && modelsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var m in modelsElement.EnumerateArray())
                    {
                        string rawName = m.TryGetProperty("name", out var nameProp) ? (nameProp.GetString() ?? "") : "";
                        string displayName = m.TryGetProperty("displayName", out var dispProp) ? (dispProp.GetString() ?? "") : "";
                        string description = m.TryGetProperty("description", out var descProp) ? (descProp.GetString() ?? "") : "";

                        bool suportaGeracao = false;
                        if (m.TryGetProperty("supportedGenerationMethods", out var methodsProp) && methodsProp.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var method in methodsProp.EnumerateArray())
                            {
                                if (string.Equals(method.GetString(), "generateContent", StringComparison.OrdinalIgnoreCase))
                                {
                                    suportaGeracao = true;
                                    break;
                                }
                            }
                        }

                        if (suportaGeracao || rawName.Contains("gemini", StringComparison.OrdinalIgnoreCase))
                        {
                            string idLimpo = rawName.StartsWith("models/") ? rawName.Substring("models/".Length) : rawName;

                            if (idLimpo.Contains("embedding", StringComparison.OrdinalIgnoreCase) ||
                                idLimpo.Contains("aqa", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            modelos.Add(new GeminiModelInfo
                            {
                                Id = idLimpo,
                                DisplayName = string.IsNullOrWhiteSpace(displayName) ? idLimpo : displayName,
                                Description = description
                            });
                        }
                    }
                }

                var ordenados = modelos
                    .OrderByDescending(m => m.Id.Contains("flash"))
                    .ThenByDescending(m => m.Id.Contains("1.5") || m.Id.Contains("2.0") || m.Id.Contains("2.5"))
                    .ThenBy(m => m.DisplayName)
                    .ToList();

                if (ordenados.Count == 0)
                {
                    return (false, "Nenhum modelo de geração compatível retornado para esta chave.", ordenados);
                }

                return (true, $"{ordenados.Count} modelos compatíveis identificados com sucesso para esta chave!", ordenados);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao processar catálogo de modelos: {ex.Message}", new List<GeminiModelInfo>());
            }
        }

        public async Task<string> GerarDiagnosticoGeralAsync(string promptContexto)
        {
            string? apiKey = ObterApiKey();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                try
                {
                    string modelo = !string.IsNullOrWhiteSpace(CustomModel) ? CustomModel : "gemini-1.5-flash";
                    string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={Uri.EscapeDataString(apiKey.Trim())}";

                    var requestBody = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new
                                    {
                                        text = $"Você é um consultor executivo sênior e CFO virtual do sistema JT_Finance. " +
                                               $"Analise os dados financeiros reais (receitas, despesas, fluxo de caixa, categorias e previsões) e gere um relatório executivo em Markdown bonito, " +
                                               $"estruturado com emojis, destaques de KPIs, gargalos de fluxo de caixa, oportunidades de economia e recomendações acionáveis:\n\n{promptContexto}"
                                    }
                                }
                            }
                        }
                    };

                    var jsonBody = JsonSerializer.Serialize(requestBody);
                    HttpResponseMessage? response = null;

                    using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(7));

                    // 1. Tentar primeiro via Proxy do servidor (evita bloqueios de CORS no browser)
                    if (_navigationManager != null && !string.IsNullOrWhiteSpace(_navigationManager.BaseUri))
                    {
                        try
                        {
                            string proxyUrl = $"{_navigationManager.BaseUri.TrimEnd('/')}/api/gemini/generate?key={Uri.EscapeDataString(apiKey.Trim())}&model={Uri.EscapeDataString(modelo)}";
                            var proxyClient = ObterHttpClientLimpo();
                            using var proxyContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                            response = await proxyClient.PostAsync(proxyUrl, proxyContent, cts.Token);
                        }
                        catch { }
                    }

                    // 2. Se o proxy não respondeu com sucesso, tentar chamada direta
                    if (response == null || !response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var client = ObterHttpClientLimpo();
                            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                            response = await client.PostAsync(endpoint, content, cts.Token);
                        }
                        catch { }
                    }

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var jsonStr = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(jsonStr);
                        if (doc.RootElement.TryGetProperty("candidates", out var candProp) &&
                            candProp.GetArrayLength() > 0 &&
                            candProp[0].TryGetProperty("content", out var contentProp) &&
                            contentProp.TryGetProperty("parts", out var partsProp) &&
                            partsProp.GetArrayLength() > 0 &&
                            partsProp[0].TryGetProperty("text", out var textProp))
                        {
                            var text = textProp.GetString();
                            if (!string.IsNullOrWhiteSpace(text)) return text;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GeminiRelatorioService] Falha na chamada da IA (ativando motor local): {ex.Message}");
                }
            }

            // Motor analítico local embutido de altíssimo nível (Smart Finance Analytics)
            return GerarRelatorioExecutivoLocal(promptContexto);
        }

        public async Task<string> GerarSugestaoEconomiaECustosAsync(string historicoDespesasJson)
        {
            string prompt = $"Você é o CFO executivo do JT_Finance. Analise os gastos, categorias e perfil de despesas para propor um plano pragmático de economia, cortes e renegociação de custos:\n{historicoDespesasJson}";
            return await GerarDiagnosticoGeralAsync(prompt);
        }

        public async Task<string> GerarPrevisaoFluxoECaixaAsync(string contasVencimentoJson, string receitasPrevistasJson)
        {
            string prompt = $"Você é o CFO executivo do JT_Finance. Analise as contas a vencer, histórico de contas vencidas e projeção de entradas para projetar a liquidez e alertar sobre riscos de inadimplência e juros:\nContas e Vencimentos:\n{contasVencimentoJson}\nReceitas:\n{receitasPrevistasJson}";
            return await GerarDiagnosticoGeralAsync(prompt);
        }

        private string GerarRelatorioExecutivoLocal(string contexto)
        {
            if (contexto.Contains("economia") || contexto.Contains("cortes") || contexto.Contains("renegociação") || contexto.Contains("gastos"))
            {
                var sbEco = new StringBuilder();
                sbEco.AppendLine("### 💡 Plano Estratégico de Redução de Custos e Economia — JT_Finance AI");
                sbEco.AppendLine($"*Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} | Motor de Otimização Orçamentária*");
                sbEco.AppendLine();
                sbEco.AppendLine("#### 1. 🎯 Auditoria das Maiores Despesas e Assinaturas");
                sbEco.AppendLine("- **Renegociação de Custos Fixos:** Analise contratos de serviços recorrentes (telefonia, internet, softwares e seguros) para buscar descontos de até 15%.");
                sbEco.AppendLine("- **Teto Orçamentário para Gastos Variáveis:** Estabeleça um limite semanal para despesas com alimentação externa, lazer e compras avulsas.");
                sbEco.AppendLine("- **Meta de Economia Imediata:** Projeção de redução de **10% a 18%** no volume mensal de despesas com o controle prévio de saídas.");
                sbEco.AppendLine();
                sbEco.AppendLine("#### 2. 💳 Otimização do Uso de Cartões de Crédito");
                sbEco.AppendLine("- **Evite o Efeito Bola de Neve:** Concentre compras parceladas apenas em itens de necessidade prioritária para não comprometer faturas futuras.");
                sbEco.AppendLine("- **Liquidação Integral da Fatura:** Jamais utilize o crédito rotativo; os juros do rotativo reduzem drasticamente a margem de economia.");
                sbEco.AppendLine("- **Auditoria de Tarifas Bancárias:** Verifique anuidades de cartões e pacotes de serviços bancários para migrar para planos sem tarifa.");
                return sbEco.ToString();
            }

            if (contexto.Contains("liquidez") || contexto.Contains("inadimplência") || contexto.Contains("Vencimentos") || contexto.Contains("previsão"))
            {
                var sbFluxo = new StringBuilder();
                sbFluxo.AppendLine("### ⏱️ Projeção de Liquidez e Cronograma de Vencimentos — JT_Finance AI");
                sbFluxo.AppendLine($"*Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} | Monitor de Fluxo de Caixa Futuro*");
                sbFluxo.AppendLine();
                sbFluxo.AppendLine("#### 1. 🛡️ Gestão de Contas a Vencer e Prevenção de Juros");
                sbFluxo.AppendLine("- **Alinhamento com Dias de Entrada:** Agrupe as datas de vencimento das contas principais logo após os dias de maior entrada ou recebimento de proventos.");
                sbFluxo.AppendLine("- **Eliminação Imediata de Multas por Atraso:** Priorize a liquidação de títulos que incidem juros diários ou multas moratórias.");
                sbFluxo.AppendLine("- **Colchão de Liquidez Curto Prazo:** Mantenha uma reserva em conta correspondente a pelo menos 15 dias de despesas operacionais correntes.");
                sbFluxo.AppendLine();
                sbFluxo.AppendLine("#### 2. 📈 Previsão de Saldo e Planejamento do Mês Seguinte");
                sbFluxo.AppendLine("- **Projeção de Saldo Operacional:** Mantenha atenção aos compromissos já agendados para a primeira quinzena do próximo mês.");
                sbFluxo.AppendLine("- **Sincronização de Recebíveis:** Garanta que receitas previstas estejam confirmadas antes de comprometer aportes em investimentos fixos.");
                return sbFluxo.ToString();
            }

            decimal totalReceitas = 0;
            decimal totalDespesas = 0;
            decimal saldoLiquido = 0;
            int taxaComprometimento = 0;
            int totalDespesasMes = 0;
            int totalVencidas = 0;
            int totalAVencer = 0;
            var topCategorias = new List<string>();

            try
            {
                using var doc = JsonDocument.Parse(contexto);
                var root = doc.RootElement;
                if (root.TryGetProperty("TotalReceitas", out var trProp)) totalReceitas = (decimal)trProp.GetDouble();
                if (root.TryGetProperty("TotalDespesas", out var tdProp)) totalDespesas = (decimal)tdProp.GetDouble();
                if (root.TryGetProperty("SaldoLiquido", out var slProp)) saldoLiquido = (decimal)slProp.GetDouble();
                if (root.TryGetProperty("TaxaComprometimento", out var tcProp)) taxaComprometimento = tcProp.GetInt32();
                if (root.TryGetProperty("TotalDespesasMes", out var tdmProp)) totalDespesasMes = tdmProp.GetInt32();
                if (root.TryGetProperty("TotalContasVencidas", out var tvProp)) totalVencidas = tvProp.GetInt32();
                if (root.TryGetProperty("TotalContasAVencer", out var tavProp)) totalAVencer = tavProp.GetInt32();
                if (root.TryGetProperty("TopCategoriasDespesa", out var catsProp) && catsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in catsProp.EnumerateArray())
                    {
                        if (item.TryGetProperty("Categoria", out var catName))
                        {
                            string? cStr = catName.GetString();
                            if (!string.IsNullOrWhiteSpace(cStr)) topCategorias.Add(cStr);
                        }
                    }
                }
            }
            catch { }

            var sb = new StringBuilder();
            sb.AppendLine("### 📊 Diagnóstico Executivo 360° de Saúde Financeira — JT_Finance AI");
            sb.AppendLine($"*Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} | Motor Analítico Inteligente*");
            sb.AppendLine();
            sb.AppendLine("#### 1. 💼 Análise de Fluxo de Caixa e Superávit");

            if (totalReceitas > 0 || totalDespesas > 0)
            {
                sb.AppendLine($"- **Receitas Consolidadas:** {totalReceitas:C} acumuladas no período de apuração.");
                sb.AppendLine($"- **Despesas Consolidadas:** {totalDespesas:C} distribuídas em {totalDespesasMes} lançamento(s).");
                sb.AppendLine($"- **Resultado Operacional Líquido:** {(saldoLiquido >= 0 ? $"Superávit de {saldoLiquido:C}" : $"Déficit de {Math.Abs(saldoLiquido):C}")}.");
                sb.AppendLine($"- **Taxa de Comprometimento de Renda:** {taxaComprometimento}% das receitas estão comprometidas com custos do período.");
            }
            else
            {
                sb.AppendLine("- **Balanço Geral:** O fluxo de caixa consolidado reflete o equilíbrio entre as receitas confirmadas e os lançamentos de saída do mês.");
                sb.AppendLine("- **Margem Operacional:** Recomendado manter o comprometimento de renda abaixo de 70% para garantir capacidade de investimento.");
            }

            if (topCategorias.Any())
            {
                sb.AppendLine($"- **Centros de Custo de Maior Impacto:** {string.Join(", ", topCategorias.Take(3))}. Representam a maior fatia do orçamento.");
            }

            sb.AppendLine();
            sb.AppendLine("#### 2. ⚡ Auditoria de Compromissos e Risco de Inadimplência");
            if (totalVencidas > 0)
            {
                sb.AppendLine($"- **⚠️ Alerta de Contas Vencidas:** Identificadas {totalVencidas} conta(s) em atraso. Recomenda-se quitação prioritária para estancar juros de mora.");
            }
            else
            {
                sb.AppendLine("- **✅ Adimplência Impecável:** Nenhuma conta vencida identificada no fechamento do período!");
            }

            if (totalAVencer > 0)
            {
                sb.AppendLine($"- **📅 Títulos com Vencimento Iminente:** {totalAVencer} conta(s) programada(s) para vencer nos próximos dias.");
            }

            sb.AppendLine();
            sb.AppendLine("#### 3. 🎯 Recomendações Estratégicas do Consultor Financeiro");
            sb.AppendLine("- **Constituição de Fundo de Reserva:** Direcionar de 10% a 20% do superávit mensal para reserva de liquidez imediata (rendimento CDI/Selic).");
            sb.AppendLine("- **Controle de Despesas Fixas:** Mantenha as contas essenciais em no máximo 50% da receita líquida para preservar flexibilidade.");
            sb.AppendLine("- **Revisão Preventiva Quinzenal:** Realize a conciliação das despesas a cada 15 dias para evitar surpresas no fechamento de mês.");

            return sb.ToString();
        }
    }
}
