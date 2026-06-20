using JT_Aplicacao;

using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Finance.Shared.Components.Modais;

using JT_UserInterface.Shared.Components;
using JT_UserInterface.Shared.Components.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Text;

namespace JT_Finance.Shared.Pages
{
    public partial class Fatura : ComponentBase
    {
        [Parameter]
        public int CartaoId { get; set; }

        [Parameter, SupplyParameterFromQuery(Name = "year")]
        public int? Year { get; set; }

        [Parameter, SupplyParameterFromQuery(Name = "month")]
        public int? Month { get; set; }

        [CascadingParameter]
        public Func<string, Task>? SetPageTitleAsync { get; set; }

        private CartaoCredito? cartao;
        private List<FaturaViewModel> faturas = new();
        private decimal totalEmAberto = 0m;
        private decimal totalPago = 0m;
        private bool isLoading = true;

        public class FaturaViewModel
        {
            public DateTime DataVencimento { get; set; }
            public decimal ValorTotal => Lancamentos.Sum(l => l.Despesa.Valor);
            public decimal ValorPago => Lancamentos.Where(l => l.DespesaCartaoCredito.ParcelaPaga == true).Sum(l => l.Despesa.Valor);
            public decimal ValorEmAberto => ValorTotal - ValorPago;
            public int QuantidadeLancamentos => Lancamentos.Count;
            public List<sCartaoDespesa> Lancamentos { get; set; } = new();
            public bool EstaExpandida { get; set; } = false;
            public bool EstaPaga => Lancamentos.All(l => l.DespesaCartaoCredito.ParcelaPaga == true);
        }
        private bool isProcessing = false;

        private Modal? pagamentoModal;
        private FaturaViewModel? faturaParaPagar;
        private DateTime dataPagamento = DateTime.Today;

        private ModalDespesa modalDespesa = new();
        private Despesa? despesaAtual;

        private DotNetObjectReference<Fatura>? _novoHotkeysRef;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _novoHotkeysRef = DotNetObjectReference.Create(this);
                    await JSRuntime.InvokeVoidAsync("JTHotkeys.enableNewHotkeys", _novoHotkeysRef);
                }
                catch { }
            }
        }

        [JSInvokable]
        public async Task OnNovoHotkey()
        {
            await InvokeAsync(() =>
            {
                AbrirModalParaNovaDespesaGeral();
                StateHasChanged();
            });
        }

        private void AbrirModalPagamento(FaturaViewModel fatura)
        {
            faturaParaPagar = fatura;
            dataPagamento = DateTime.Today;
            pagamentoModal.Open();
        }

        private void AbrirModalParaNovaDespesaGeral()
        {
            if (cartao is null) return;

            despesaAtual = new Despesa
            {
                Descricao = string.Empty,
                Valor = 0m,
                DataDespesa = DateTime.Today,
                EhDespesaCartaoCredito = true,

            };


            despesaAtual.DetalhesCartaoCredito.IdCartao = cartao.Id;
            despesaAtual.DetalhesCartaoCredito.QuantidadeParcelas = 1;
            despesaAtual.DetalhesCartaoCredito.DataVencimento =
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1)
                    .AddDays(cartao.DiaVencimento - 1);

            modalDespesa.Title = $"Nova Despesa - {cartao.Nome}";
            modalDespesa.Open();
        }

        private void AbrirModalParaNovaDespesaNaFatura(FaturaViewModel fatura)
        {
            if (cartao is null) return;

            despesaAtual = new Despesa
            {
                Descricao = string.Empty,
                Valor = 0m,
                DataDespesa = DateTime.Today,
                EhDespesaCartaoCredito = true
            };

            despesaAtual.DetalhesCartaoCredito.IdCartao = cartao.Id;
            despesaAtual.DetalhesCartaoCredito.QuantidadeParcelas = 1;
            despesaAtual.DetalhesCartaoCredito.DataVencimento =
                new DateTime(fatura.DataVencimento.Year, fatura.DataVencimento.Month, cartao.DiaVencimento);

            modalDespesa.Title = $"Nova Despesa nesta Fatura - {cartao.Nome}";
            modalDespesa.Open();
        }

        private async Task ConfirmarPagamento()
        {
            if (isProcessing || faturaParaPagar == null)
            {
                return;
            }

            isProcessing = true;
            await InvokeAsync(StateHasChanged);

            try
            {
                bool sucesso = await CartaoService.PagarFatura(faturaParaPagar.Lancamentos, dataPagamento);

                if (sucesso)
                {
                    await RecarregarFaturasAsync();
                    ToastService.ShowSuccess("Sucesso!", "Fatura paga com a data selecionada.");
                }
                else
                {
                    ToastService.ShowError("Erro!", "Não foi possível pagar a fatura. Verifique os logs.");
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Erro Inesperado!", "Ocorreu uma falha ao processar o pagamento.");
                Log.RegistrarErro(ex);
            }
            finally
            {
                isProcessing = false;
                if (pagamentoModal != null)
                {
                    await pagamentoModal.Close();
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (cartao is not null)
            {
                await RecarregarFaturasAsync();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            await Task.Yield();

            cartao = new CartaoCredito(CartaoId);

            await RecarregarFaturasAsync();

            if (SetPageTitleAsync != null)
            {
                var titulo = cartao != null ? $"Faturas do Cartão - {cartao.Nome}" : "Faturas do Cartão";
                if (Year.HasValue && Month.HasValue)
                    titulo += $" ({new DateTime(Year.Value, Month.Value, 1):MMMM yyyy})";
                await SetPageTitleAsync.Invoke(titulo);
            }

            isLoading = false;
        }

        private async Task RecarregarFaturasAsync()
        {
            if (cartao != null && cartao.Id > 0)
            {
                var todasDespesas = await CartaoService.ObterDespesas(cartao.Id) ?? new List<sCartaoDespesa>();

                faturas = todasDespesas
                    .GroupBy(d =>
                    {
                        var dv = d.DespesaCartaoCredito.DataVencimento;
                        return (dv.Year, dv.Month);
                    })
                    .Select(grupo => new FaturaViewModel
                    {
                        DataVencimento = new DateTime(grupo.Key.Year, grupo.Key.Month, cartao.DiaVencimento),
                        Lancamentos = grupo.ToList()
                    })
                    .OrderByDescending(f => f.DataVencimento)
                    .ToList();

                if (Year.HasValue && Month.HasValue)
                {
                    faturas = faturas
                        .Where(f => f.DataVencimento.Year == Year.Value && f.DataVencimento.Month == Month.Value)
                        .ToList();

                    foreach (var f in faturas) f.EstaExpandida = true;
                }

                // Totais consolidados
                totalPago = faturas.Sum(f => f.ValorPago);
                totalEmAberto = faturas.Sum(f => f.ValorEmAberto);
            }

            await InvokeAsync(StateHasChanged);
        }

        private void AlternarVisibilidadeFatura(FaturaViewModel fatura)
        {
            fatura.EstaExpandida = !fatura.EstaExpandida;
        }

        private async Task HandleCancelarFormularioModal(MouseEventArgs args)
        {
            await pagamentoModal.Close();
        }

        private async Task HandleCancelarModalDespesa()
        {
            despesaAtual = null;
            await modalDespesa.Close();
        }

        private async Task OnDespesaModalClose()
        {
            despesaAtual = null;
            await RecarregarFaturasAsync();
        }

        private async Task Voltar()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("BlazorInterop.goBack", "/cartoes-credito");
            }
            catch
            {
                NavigationManager.NavigateTo("cartoes-credito");
            }
        }

        public void Dispose()
        {
            try { _ = JSRuntime.InvokeVoidAsync("JTHotkeys.disableNewHotkeys"); } catch { }
            _novoHotkeysRef?.Dispose();
        }
    }
}
