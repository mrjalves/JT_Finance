using JT_UserInterface.Shared.Components.Core;
using JT_Classes.structs;

using JT_UserInterface.Shared.Components;
using JT_UserInterface.Shared.Components.Core.Charts;
using JT_UserInterface.Shared.Components.Core.Charts;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;

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

        // --- Dados para os Gráficos ---
        private List<string> despesaTiposLabels = new();
        private List<ChartDataset> datasetsGraficoPorTipo = new();
        private List<string> despesaCategoriasLabels = new();
        private List<ChartDataset> datasetsGraficoPorCategoria = new();
        private List<sDespesa> DespesasVencidas = new();
        private List<sDespesa> DespesaAVencer = new();
        private List<sDespesa> DespesaMes = new();
        protected override async Task OnInitializedAsync()
        {
            if (SetPageTitleAsync != null)
            {
                await SetPageTitleAsync.Invoke("Dashboard");
            }
            await LoadDashboardData();
            DespesaAVencer = await DespesaService.ObterDespesasAVencerAsync();
             DespesasVencidas = await DespesaService.ObterDespesasVencidasAsync();
        }

        private async Task LoadDashboardData()
        {
            isLoading = true;
            await InvokeAsync(StateHasChanged);

            // Usar Task.Run para não bloquear a UI enquanto os dados são buscados
            await Task.Run(async () =>
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
                new() {
                    Label = "Total por Tipo",
                    Data = despesasPorTipo.Select(d => d.ValorTotal).ToList()
                }
            };

                // --- GRÁFICO DE CATEGORIA (ROSCA) ---
                var despesasPorCategoria = await DespesaService.GetDespesaPorCategoria(mesReferencia);
                despesaCategoriasLabels = despesasPorCategoria.Select(d => d.Categoria).ToList();
                datasetsGraficoPorCategoria = new List<ChartDataset>
            {
                new() {
                    Data = despesasPorCategoria.Select(d => d.ValorTotal).ToList()
                }
            };
            });

            isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task MudarMes(int mesesParaAdicionar)
        {
            mesReferencia = mesReferencia.AddMonths(mesesParaAdicionar);
            await LoadDashboardData();
        }

        private async Task MudarMesAnterior() => await MudarMes(-1);
        private async Task MudarMesProximo() => await MudarMes(1);


        private JT_UserInterface.Shared.Components.Core.Modal modalDetalhes;
        private List<sDespesa> despesasParaExibir = new();
        private void AbrirModalDetalhes(string titulo, List<sDespesa> despesas)
        {
            despesasParaExibir = despesas;
            modalDetalhes.Title = titulo;
            modalDetalhes.Open();
        }
    }
}



