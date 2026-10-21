using JT_UserInterface.Shared.Components.Core;
using JT_Classes.structs;
using JT_UserInterface.Shared.Components;
using JT_UserInterface.Shared.Components.Core.Charts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Pages
{
    public partial class Home : ComponentBase
    {
        [CascadingParameter]
        public Func<string, Task>? SetPageTitleAsync { get; set; }

        // --- Estado da Página ---
        private DateTime mesReferencia = DateTime.Now;
        private bool isLoading = true;

        // --- Dados para os Cards ---
        private decimal totalReceitas;
        private decimal totalDespesas;
        private decimal saldoAtual;

        // --- Indicadores Calculados ---
        private int TaxaComprometimento => totalReceitas > 0 ? (int)Math.Min(100, Math.Round((totalDespesas / totalReceitas) * 100)) : (totalDespesas > 0 ? 100 : 0);
        private bool IsMesAtual => mesReferencia.Month == DateTime.Now.Month && mesReferencia.Year == DateTime.Now.Year;

        // --- Dados para os Gráficos e Painéis ---
        private List<string> despesaTiposLabels = new();
        private List<ChartDataset> datasetsGraficoPorTipo = new();
        private List<string> despesaCategoriasLabels = new();
        private List<ChartDataset> datasetsGraficoPorCategoria = new();
        private List<sDespesa> DespesasVencidas = new();
        private List<sDespesa> DespesaAVencer = new();
        private List<sDespesa> DespesaMes = new();

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authenticationState.User.Identity?.IsAuthenticated != true)
            {
                NavigationManager.NavigateTo("/login?returnUrl=%2Fhome");
                return;
            }

            if (SetPageTitleAsync != null)
            {
                await SetPageTitleAsync.Invoke("Dashboard");
            }

            await RecarregarMetricasAsync();
        }

        private async Task<bool> LoadDashboardData()
        {
            isLoading = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                // --- CARREGA DADOS DOS CARDS ---
                totalReceitas = await ReceitaService.GetTotalMes(mesReferencia);
                totalDespesas = await DespesaService.GetTotalMes(mesReferencia);
                saldoAtual = totalReceitas - totalDespesas;

                DespesaMes = await DespesaService.ObterDespesasMesAsync(mesReferencia);

                // --- GRÁFICO DE TIPO (BARRAS) ---
                var despesasPorTipo = await DespesaService.GetDespesaPorTipo(mesReferencia);
                despesaTiposLabels = despesasPorTipo.Select(d => d.Tipo).ToList();
                datasetsGraficoPorTipo = new List<ChartDataset>
                {
                    new()
                    {
                        Label = "Total por Tipo",
                        Data = despesasPorTipo.Select(d => d.ValorTotal).ToList()
                    }
                };

                // --- GRÁFICO DE CATEGORIA (ROSCA) ---
                var despesasPorCategoria = await DespesaService.GetDespesaPorCategoria(mesReferencia);
                despesaCategoriasLabels = despesasPorCategoria.Select(d => d.Categoria).ToList();
                datasetsGraficoPorCategoria = new List<ChartDataset>
                {
                    new()
                    {
                        Data = despesasPorCategoria.Select(d => d.ValorTotal).ToList()
                    }
                };

                return true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                NavigationManager.NavigateTo("/login?returnUrl=%2Fhome");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO DASHBOARD] Falha ao carregar dados: {ex.Message}");
                return true;
            }
            finally
            {
                isLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task RecarregarMetricasAsync()
        {
            if (!await LoadDashboardData())
                return;

            DespesaAVencer = await DespesaService.ObterDespesasAVencerAsync();
            DespesasVencidas = await DespesaService.ObterDespesasVencidasAsync();
        }

        private async Task MudarMes(int mesesParaAdicionar)
        {
            mesReferencia = mesReferencia.AddMonths(mesesParaAdicionar);
            await RecarregarMetricasAsync();
        }

        private async Task MudarMesAnterior() => await MudarMes(-1);
        private async Task MudarMesProximo() => await MudarMes(1);

        private async Task IrParaMesAtualAsync()
        {
            mesReferencia = DateTime.Now;
            await RecarregarMetricasAsync();
        }

        private string ObterStatusVencimento(DateTime dataVencimento)
        {
            var dias = (DateTime.Today - dataVencimento.Date).Days;
            if (dias == 0) return "Vence hoje";
            if (dias > 0) return $"Atrasada há {dias} dia(s)";
            return $"Vence em {Math.Abs(dias)} dia(s)";
        }

        private JT_UserInterface.Shared.Components.Core.Modal modalDetalhes;
        private List<sDespesa> despesasParaExibir = new();
        private void AbrirModalDetalhes(string titulo, List<sDespesa> despesas)
        {
            despesasParaExibir = despesas ?? new List<sDespesa>();
            modalDetalhes.Title = titulo;
            modalDetalhes.Open();
        }
    }
}
