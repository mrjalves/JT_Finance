using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Servicos.Financeiro;

using JT_UserInterface.Shared.Services.Comum;
using JT_Aplicacao.Classes;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JT_Finance.Shared.Components.Grids
{
    public partial class GridCartoesCredito : ComponentBase
    {
        [Parameter]
        public List<sCartao> Cartoes { get; set; } = new();

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback<int> OnEditar { get; set; }

        [Parameter]
        public EventCallback<int> OnRemover { get; set; }

        [Inject] public JT_UserInterface.Shared.Services.Comum.HotkeysDisplayService HotkeysDisplay { get; set; } = default!;
        [Inject] public FiltroService FiltroService { get; set; } = default!;

        private List<sCartao> listaTodosCartoes = new();
        private List<sCartao> cartoesFiltrados = new();
        private string _termoPesquisa = "";
        private List<Filtro> _filtrosAplicados = new();

        public IEnumerable<sCartao> CartoesFiltrados => cartoesFiltrados;

        private const string KeyNome = nameof(sCartao.Nome);
        private const string KeyLimite = nameof(sCartao.Limite);
        private const string KeyVencimento = "Vencimento";
        private const string KeyFechamento = "Fechamento";
        private const string KeyStatus = "Status";
        private const string KeyFaturaMes = "FaturaMes";


        private IReadOnlyDictionary<string, Func<sCartao, IComparable?>> SortSelectors =>
            new Dictionary<string, Func<sCartao, IComparable?>>
            {
                [KeyNome] = c => c.Nome,
                [KeyLimite] = c => c.Limite,
                [KeyVencimento] = c => c.DiaVencimento,
                [KeyFechamento] = c => c.DiaFechamento,
                [KeyStatus] = c => c.Ativo,
                [KeyFaturaMes] = c => _totaisFaturaMes.TryGetValue(c.Id, out var v) ? v : 0m
            };


        // Totais (já existentes)
        private int QtdCartoes => CartoesFiltrados.Count();
        private int QtdAtivos => CartoesFiltrados.Count(c => c.Ativo);
        private int QtdInativos => QtdCartoes - QtdAtivos;
        private decimal LimiteTotal => CartoesFiltrados.Sum(c => c.Limite);

        // Novos dicionários de totais
        private readonly Dictionary<int, decimal> _totaisEmAberto = new();
        private readonly Dictionary<int, decimal> _totaisFaturaMes = new();

        private decimal TotalGeralEmAberto => _totaisEmAberto.Values.DefaultIfEmpty(0m).Sum();
        private decimal TotalGeralFaturaMes => _totaisFaturaMes.Values.DefaultIfEmpty(0m).Sum();

        protected override async Task OnParametersSetAsync()
        {
            await HotkeysDisplay.InicializarAsync();
            var novosCartoes = Cartoes ?? new List<sCartao>();
            if (!ReferenceEquals(listaTodosCartoes, novosCartoes))
            {
                listaTodosCartoes = novosCartoes.ToList();
                AplicarVisao();
            }
            ConfigurarFiltros();
            await base.OnParametersSetAsync();
            await CarregarTotaisFaturaMesAsync();
            await CarregarTotaisEmAbertoAsync();
        }

        private void ConfigurarFiltros()
        {
            FiltroService.RegistrarFiltros(CartaoCredito.GetFiltros(), AplicarFiltros, AplicarPesquisa);
        }

        private Task AplicarPesquisa(string term)
        {
            _termoPesquisa = term ?? "";
            AplicarVisao();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task AplicarFiltros(List<Filtro> filtros)
        {
            _filtrosAplicados = filtros ?? new List<Filtro>();
            AplicarVisao();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void AplicarVisao()
        {
            IEnumerable<sCartao> visao = listaTodosCartoes;

            if (!string.IsNullOrWhiteSpace(_termoPesquisa))
            {
                var termo = _termoPesquisa.Trim();
                visao = visao.Where(c =>
                    (c.Nome != null && c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))
                    || c.Limite.ToString(System.Globalization.CultureInfo.CurrentCulture).Contains(termo, StringComparison.OrdinalIgnoreCase)
                    || c.DiaVencimento.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase)
                    || c.DiaFechamento.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase)
                    || c.Ativo.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var filtro in _filtrosAplicados.Where(f => f.Value != null && !string.IsNullOrWhiteSpace(f.Value?.ToString())))
            {
                var valor = filtro.Value?.ToString();
                switch (filtro.Label)
                {
                    case "Nome":
                        visao = visao.Where(c => c.Nome != null && c.Nome.Contains(valor!, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Limite":
                        if (decimal.TryParse(valor, out var limite))
                        {
                            visao = visao.Where(c => c.Limite == limite);
                        }
                        break;
                    case "Situação":
                        var ativo = valor == "1";
                        visao = visao.Where(c => c.Ativo == ativo);
                        break;
                }
            }

            cartoesFiltrados = visao.ToList();
        }

        private async Task CarregarTotaisFaturaMesAsync()
        {
            _totaisFaturaMes.Clear();
            if (Cartoes is null || Cartoes.Count == 0) return;

            var hoje = DateTime.Today;
            var inicio = new DateTime(hoje.Year, hoje.Month, 1);
            var fim = inicio.AddMonths(1).AddDays(-1);

            foreach (var c in Cartoes)
            {
                var lanc = await CartaoService.ObterDespesas(c.Id) ?? new List<sCartaoDespesa>();
                var totalMes = lanc
                    .Where(l => l.DespesaCartaoCredito != null
                                && l.DespesaCartaoCredito.DataVencimento >= inicio
                                && l.DespesaCartaoCredito.DataVencimento <= fim)
                                .Sum(l => l.Despesa != null ? l.Despesa.Valor : 0m);
                _totaisFaturaMes[c.Id] = totalMes;
                await Task.Yield();
            }
            StateHasChanged();
        }

        private async Task CarregarTotaisEmAbertoAsync()
        {
            _totaisEmAberto.Clear();
            if (Cartoes is null || Cartoes.Count == 0) return;

            foreach (var c in Cartoes)
            {
                var lanc = await CartaoService.ObterDespesas(c.Id) ?? new List<sCartaoDespesa>();
                var totalAberto = lanc
                    .Where(l => l.DespesaCartaoCredito.ParcelaPaga != true)
                    .Sum(l => l.Despesa != null ? l.Despesa.Valor : 0m);

                _totaisEmAberto[c.Id] = totalAberto;
                await Task.Yield();
            }
            StateHasChanged();
        }

        private decimal GetTotalEmAberto(int cartaoId) =>
            _totaisEmAberto.TryGetValue(cartaoId, out var v) ? v : 0m;

        private decimal GetTotalFaturaMes(int cartaoId) =>
            _totaisFaturaMes.TryGetValue(cartaoId, out var v) ? v : 0m;

        private void VerFatura(int cartaoId)
        {
            NavigationManager.NavigateTo($"/fatura/{cartaoId}");
        }
    }
}

