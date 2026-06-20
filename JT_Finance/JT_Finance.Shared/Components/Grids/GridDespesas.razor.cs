using JT_Aplicacao.Classes;
using JT_UserInterface.Shared.Services.Comum;
using JT_Aplicacao;

using JT_Classes;
using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Servicos;
using JT_Servicos.Financeiro;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;

namespace JT_Finance.Shared.Components.Grids
{
    public partial class GridDespesas : ComponentBase
    {
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public EventCallback<sDespesa> OnDuplicar { get; set; }
        [Parameter] public EventCallback<int> OnEditar { get; set; }
        [Parameter] public EventCallback<int> OnRemover { get; set; }
        [Parameter] public bool HideActions { get; set; } = false;
        [Parameter] public List<sDespesa>? Despesas { get; set; }

        private List<sDespesa> listaTodasDespesas = new();
        

        private const string KeyDescricao = nameof(sDespesa.Descricao);
        private const string KeyValor = nameof(sDespesa.Valor);
        private const string KeyData = nameof(sDespesa.DataDespesa);
        private const string KeyVencimento = "Vencimento";
        private const string KeyCategoria = "Categoria";
        private const string KeyTipo = "Tipo";
        private const string KeyCartaoParcela = "CartaoParcela";
        private const string KeyPaga = "Paga";

        private IEnumerable<sDespesa> ItensBase =>
            (Despesas ?? listaTodasDespesas) ?? Enumerable.Empty<sDespesa>();

        // Totais
        private int QtdDespesas => ItensBase.Count();
        private decimal TotalDespesas => ItensBase.Sum(d => d.Valor);
        private decimal TotalPagas => ItensBase.Where(d => d.DespesaPaga).Sum(d => d.Valor);
        private decimal TotalPendentes => TotalDespesas - TotalPagas;

        private static readonly IReadOnlyDictionary<string, Func<sDespesa, IComparable?>> SortSelectors
            = new Dictionary<string, Func<sDespesa, IComparable?>>
            {
                [KeyDescricao] = d => d.Descricao,
                [KeyValor] = d => d.Valor,
                [KeyData] = d => d.DataDespesa,
                [KeyCategoria] = d => d.CategoriaStruct.Nome,
                [KeyTipo] = d => d.TipoStruct.Nome,
                [KeyPaga] = d => d.DespesaPaga,
                [KeyVencimento] = d => (d.EhDespesaCartaoCredito && d.DespCartaoCredito.DataVencimento != default)
                ? d.DespCartaoCredito.DataVencimento
                : DateTime.MaxValue,
                [KeyCartaoParcela] = d => d.EhDespesaCartaoCredito && !string.IsNullOrEmpty(d.Cartao.Nome)
                ? $"{d.Cartao.Nome}|{d.DespCartaoCredito.NumeroParcela:000000}"
                : "~"
            };

        protected override async Task OnInitializedAsync()
        {
            DespesaService.OnDataChanged += AtualizarDadosDaPagina;
            await CarregarDependencias();
            ConfigurarFiltros();
        }

        private async void AtualizarDadosDaPagina()
        {
            await InvokeAsync(async () =>
            {
                await CarregarDependencias();
                StateHasChanged();
            });
        }

        private async Task CarregarDependencias()
        {
            IsLoading = true;
            await Task.WhenAll(
                CarregarDespesas()
            );
            IsLoading = false;
        }

        private async Task CarregarDespesas(string whereClause = "")
        {
            if (whereClause != "")
                Despesas = await DespesaService.ObterDespesasAsync(whereClause);
            else
                listaTodasDespesas = await DespesaService.ObterTodasDespesasAsync();
        }

        

        private void ConfigurarFiltros()
        {
            var filtros = Despesa.GetFiltros();
            FiltroService.RegistrarFiltros(filtros, AplicarFiltros, AplicarPesquisa);
        }

        private Task AplicarPesquisa(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Despesas = new List<sDespesa>(listaTodasDespesas);
            }
            else
            {
                // Otimização: Evitar chamadas desnecessárias ao LevenshteinDistance
                var lowerSearchTerm = searchTerm.ToLower();
                Despesas = listaTodasDespesas
                   .Where(d => d.Descricao.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        || (Math.Abs(d.Descricao.Length - searchTerm.Length) <= 2
                            && Filtro.LevenshteinDistance(d.Descricao.ToLower(), lowerSearchTerm) <= 2))
                   .ToList();
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task AplicarFiltros(List<Filtro> filtros)
        {
            Despesas = await new JT_Servicos.Financeiro.DespesaService().Filter(filtros);
            StateHasChanged();
        }

        // --- Ações adicionadas ---
        private async Task PagarDespesaAsync(sDespesa despesa)
        {
            var entidade = despesa.ToEntity();
            entidade.DespesaPaga = true;
            entidade.DataPagamento = DateTime.Today;
            entidade.EhDespesaCartaoCredito = false;

            await DespesaService.SalvarDespesaAsync(entidade);
        }

        private void AbrirFaturaDoCartao(sDespesa despesa)
        {
            if (despesa.EhDespesaCartaoCredito && despesa.DespCartaoCredito.IdCartao > 0)
            {
                var dv = despesa.DespCartaoCredito.DataVencimento;
                NavigationManager.NavigateTo($"/fatura/{despesa.DespCartaoCredito.IdCartao}/{dv.Year}-{dv.Month}");
            }
        }
    }
}



