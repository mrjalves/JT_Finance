using JT_Classes.Financeiro;
using JT_Classes.structs;
using JT_Finance.Shared.Components.Modais;
using JT_UserInterface.Shared.Components;
using JT_UserInterface.Shared.Components.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Pages
{
    public partial class Despesas : ComponentBase
    {
        [CascadingParameter]
        public Func<string, Task>? SetPageTitleAsync { get; set; }

        private bool isLoading = true;
        private ModalDespesa modalDespesa = new();
        private int? idDespesaParaRemover;
        private ConfirmDialog confirmDeleteDialog = new();
        private Despesa? despesaAtual;

        private List<sDespesa> despesasFiltradas = new();

        private DotNetObjectReference<Despesas>? _novoHotkeysRef;

        [Inject] public JT_Classes.Acessos.CurrentUserState UserState { get; set; } = default!;
        [Inject] public JT_Servicos.Comum.ConfiguracaoService ConfiguracaoService { get; set; } = default!;
        [Inject] public JT_UserInterface.Shared.Services.Comum.HotkeysDisplayService HotkeysDisplay { get; set; } = default!;

        public string NovoHotkeySufixo => HotkeysDisplay.ComSufixo("novo");

        private int _pageSize = 10;

        protected override async Task OnInitializedAsync()
        {
            await HotkeysDisplay.InicializarAsync();

            if (SetPageTitleAsync != null)
            {
                await SetPageTitleAsync.Invoke("Despesas");
            }

            var config = await ConfiguracaoService.GetByUsuarioIdAsync(UserState.UsuarioID);
            if (config != null && config.ItensPorPagina > 0)
            {
                _pageSize = config.ItensPorPagina;
            }

            DespesaService.OnDataChanged += AtualizarDadosDaPagina;
            await CarregarDependencias();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try { await JSRuntime.InvokeVoidAsync("JTHotkeys.enableFilterHotkeys"); } catch { }
                try { await JSRuntime.InvokeVoidAsync("JTHotkeys.enableSearchHotkeys"); } catch { }
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
                AbrirModalParaNovaDespesa();
                StateHasChanged();
            });
        }

        [JSInvokable]
        public async Task HandleAcaoHotkey(string acao)
        {
            await InvokeAsync(async () =>
            {
                switch (acao)
                {
                    case "novo":
                        AbrirModalParaNovaDespesa();
                        break;
                    case "editar":
                        if (despesasFiltradas.Count > 0)
                        {
                            await AbrirModalParaEditarDespesa(despesasFiltradas[0].Id);
                        }
                        break;
                    case "deletar":
                        if (despesasFiltradas.Count > 0)
                        {
                            ConfirmarRemocaoDespesa(despesasFiltradas[0].Id);
                        }
                        break;
                    case "salvar":
                        if (despesaAtual != null)
                        {
                            await modalDespesa.Submit();
                        }
                        break;
                    case "cancelar":
                        if (despesaAtual != null)
                        {
                            await HandleCancelarFormularioModal();
                        }
                        break;
                }
                StateHasChanged();
            });
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
            isLoading = true;
            await Task.WhenAll(
                CarregarDespesas()
            );
            isLoading = false;
        }

        private async Task CarregarDespesas(string whereClause = "")
        {
            despesasFiltradas = await DespesaService.ObterTodasDespesasAsync();
        }

        private async Task DuplicarDespesa(sDespesa Despesa)
        {
            await DespesaService.DuplicarDespesaAsync(Despesa.ToEntity());
        }

        private async Task EfetuarRemocaoDespesa()
        {
            if (idDespesaParaRemover.HasValue)
            {
                await DespesaService.RemoverDespesaAsync(idDespesaParaRemover.Value);
                idDespesaParaRemover = null;
            }
        }

        private void AbrirModalParaNovaDespesa()
        {
            // new Despesa() vazio não bate no banco, então é seguro.
            despesaAtual = new Despesa();
            modalDespesa.Title = "Nova Despesa";
            modalDespesa.Open();
        }

        // CORREÇÃO AQUI: Alterado para async Task e usando o serviço para buscar os dados via API
        private async Task AbrirModalParaEditarDespesa(int id)
        {
            isLoading = true; // Opcional: mostrar carregamento caso a busca demore
            
            // Busca o objeto usando a API em vez de forçar a conexão direta pelo construtor
            var despesaCarregada = await DespesaService.ObterDespesaPorIdAsync(id);
            
            despesaAtual = despesaCarregada ?? new Despesa(); 
            modalDespesa.Title = "Editar Despesa";
            modalDespesa.Open();
            
            isLoading = false;
        }

        private async Task HandleCancelarFormularioModal()
        {
            despesaAtual = null;
            await modalDespesa.Close();
        }

        private void ResetarEstadoAposModal()
        {
            despesaAtual = null;
        }

        private void ConfirmarRemocaoDespesa(int id)
        {
            idDespesaParaRemover = id;
            confirmDeleteDialog.Show("Confirmar Remoção", "Deseja realmente remover esta despesa?");
        }

        public void Dispose()
        {
            DespesaService.OnDataChanged -= AtualizarDadosDaPagina;
            try { _ = JSRuntime.InvokeVoidAsync("JTHotkeys.disableFilterHotkeys"); } catch { }
            try { _ = JSRuntime.InvokeVoidAsync("JTHotkeys.disableSearchHotkeys"); } catch { }
            try { _ = JSRuntime.InvokeVoidAsync("JTHotkeys.disableNewHotkeys"); } catch { }
            _novoHotkeysRef?.Dispose();
        }
    }
}
