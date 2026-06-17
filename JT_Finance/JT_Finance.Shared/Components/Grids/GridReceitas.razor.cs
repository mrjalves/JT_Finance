using JT_Classes.Comum;
using JT_Aplicacao;

using JT_Classes;
using JT_Classes.Financeiro;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;

namespace JT_Finance.Shared.Components.Grids
{
    public partial class GridReceitas : ComponentBase
    {
        #region PARAMETROS
        [Parameter]
        public List<Receita> Receitas { get; set; } = new();

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<Receita> OnEditar { get; set; }

        [Parameter]
        public EventCallback<int> OnRemover { get; set; }
        [Parameter]
        public EventCallback<Receita> OnDuplicar { get; set; }
        #endregion

        private List<Categoria> TiposReceita = new();
        private List<Categoria> CategoriasReceita = new();
        private List<Receita> listaTodasReceitas = new();

        private const string KeyDescricao = nameof(Receita.Descricao);
        private const string KeyValor = nameof(Receita.Valor);
        private const string KeyData = nameof(Receita.DataReceita);
        private const string KeyCategoria = "Categoria";
        private const string KeyTipo = "Tipo";
        private const string KeyRecebida = "Recebida";

        private IEnumerable<Receita> ItensBase =>
            (Receitas != null && Receitas.Count > 0 ? Receitas : listaTodasReceitas) ?? Enumerable.Empty<Receita>();

        // Totais
        private int QtdReceitas => ItensBase.Count();
        private decimal TotalReceitas => ItensBase.Sum(r => r.Valor);
        private decimal TotalRecebidas => ItensBase.Where(r => r.ReceitaRecebida).Sum(r => r.Valor);
        private decimal TotalPendentes => TotalReceitas - TotalRecebidas;

        private IReadOnlyDictionary<string, Func<Receita, IComparable?>> SortSelectors;

        protected override async Task OnInitializedAsync()
        {
            ReceitaService.OnDataChanged += AtualizarDadosDaPagina;
            await CarregarDependencias();
            ConfigurarFiltros();
            InicializarSortSelectors();
        }

        private void InicializarSortSelectors()
        {
            SortSelectors = new Dictionary<string, Func<Receita, IComparable?>>
            {
                [KeyDescricao] = r => r.Descricao,
                [KeyValor] = r => r.Valor,
                [KeyData] = r => r.DataReceita,
                [KeyCategoria] = r => CategoriasReceita.FirstOrDefault(c => c.Id == r.Categoria)?.Descricao ?? string.Empty,
                [KeyTipo] = r => TiposReceita.FirstOrDefault(c => c.Id == r.TipoReceita)?.Descricao ?? string.Empty,
                [KeyRecebida] = r => r.ReceitaRecebida
            };
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
                CarregarReceitas(),
                CarregarOpcoes()
            );
            IsLoading = false;
        }

        private async Task CarregarReceitas(string whereClause = "")
        {
            if (whereClause != "")
                Receitas = await ReceitaService.ObterReceitasAsync(whereClause);
            else
                listaTodasReceitas = await ReceitaService.ObterTodasReceitasAsync();
        }

        private async Task CarregarOpcoes()
        {
            // TiposReceita = await CategoriaService.GetTipoCategoriasReceitas();
            // CategoriasReceita = await CategoriaService.GetCategoriasReceitas();
        }

        private void ConfigurarFiltros()
        {
            var filtros = Receita.GetFiltros();
            FiltroService.RegistrarFiltros(filtros, AplicarFiltros, AplicarPesquisa);
        }

        private Task AplicarPesquisa(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Receitas = new List<Receita>(listaTodasReceitas);
            }
            else
            {
                // Otimização: Evitar chamadas desnecessárias ao LevenshteinDistance
                var lowerSearchTerm = searchTerm.ToLower();
                Receitas = listaTodasReceitas
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
            var whereConditions = new List<string>();
            var dataInicio = filtros.FirstOrDefault(f => f.Label == "Data Início")?.Value as DateTime?;
            var dataFim = filtros.FirstOrDefault(f => f.Label == "Data Fim")?.Value as DateTime?;

            if (dataInicio.HasValue && dataFim.HasValue) whereConditions.Add($"DataReceita >= '{dataInicio}'");
            else if (dataInicio.HasValue) whereConditions.Add($"DataReceita = '{dataInicio}'");
            if (dataFim.HasValue) whereConditions.Add($"DataReceita <= '{dataFim}'");

            foreach (var filtro in filtros.Where(f => f.Value != null && !string.IsNullOrEmpty(f.Value.ToString()) && f.Label != "Data Início" && f.Label != "Data Fim"))
            {
                whereConditions.Add($"{filtro.ColumnName} = {filtro.Value}");
            }

            string whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";
            await CarregarReceitas(whereClause);
            StateHasChanged();
        }

        // Nova ação: marcar como recebida
        private async Task MarcarComoRecebidaAsync(Receita receita)
        {
            if (receita is null || receita.ReceitaRecebida) return;

            receita.ReceitaRecebida = true;
            await ReceitaService.SalvarReceitaAsync(receita);
        }
    }
}

