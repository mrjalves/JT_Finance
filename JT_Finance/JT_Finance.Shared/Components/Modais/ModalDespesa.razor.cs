using JT_Aplicacao;

using JT_Classes;
using JT_Classes.Financeiro;
using JT_Classes.structs;

using JT_Finance.Shared.Services;

using JT_UserInterface.Shared.Components;
using JT_UserInterface.Shared.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;

namespace JT_Finance.Shared.Components.Modais
{
    public partial class ModalDespesa : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public Despesa? Despesa { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private List<sCategoria> TiposDespesa = new();
        private List<sCategoria> CategoriasDespesa = new();
        private List<sCartao> CartoesCredito = new();
        private bool showNewTipoForm = false;
        private bool showNewCategoriaForm = false;
        private string novaCategoriaDescricao = string.Empty;
        private Modal ModalRef = new();

        protected override async Task OnInitializedAsync()
        {
            await CarregarOpcoes();
            await CarregarCartoesDeCredito();
            StateHasChanged();
        }
        private async Task CarregarCartoesDeCredito()
        {
            CartoesCredito = await CartoesService.ObterTodosCartoesAsync();
        }
      
        private async Task CarregarOpcoes()
        {
            try
            {
                TiposDespesa = await CategoriaService.ListarPorPapelAsync("Despesa", "TipoDespesa");
                CategoriasDespesa = await CategoriaService.ListarPorPapelAsync("Despesa", "Categoria");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar opções: {ex.Message}");
                TiposDespesa = new List<sCategoria>();
                CategoriasDespesa = new List<sCategoria>();
            }
        }

        private void ToggleInlineForm(string tipo)
        {
            novaCategoriaDescricao = string.Empty;
            if (tipo == "TipoDespesa")
            {
                showNewTipoForm = !showNewTipoForm;
                showNewCategoriaForm = false;
            }
            else
            {
                showNewCategoriaForm = !showNewCategoriaForm;
                showNewTipoForm = false;
            }
        }

        private async Task SalvarNovaCategoria(string tipo)
        {
            if (string.IsNullOrWhiteSpace(novaCategoriaDescricao)) return;

            var novaCategoria = new Categoria
            {
                TipoEntidade = "Despesa",
                Nome = tipo,
                Descricao = novaCategoriaDescricao,
                Status = StatusObjeto.Novo
            };

            var categoriasalva = await CategoriaService.SalvarCategoriaAsync(novaCategoria);
            if (categoriasalva != null)
            {
                if (tipo == "tipodespesa")
                {
                    Despesa.TipoDespesa = categoriasalva;
                    showNewTipoForm = false;
                }
                else
                {
                    Despesa.Categoria = categoriasalva;
                    showNewCategoriaForm = false;
                }
                novaCategoriaDescricao = string.Empty;
            }
        }

        private async Task HandleSubmit()
        {
            if (Despesa != null)
            {
                bool sucesso = await DespesaService.SalvarDespesaAsync(Despesa);
                if (sucesso)
                {
                    await Close();
                }
            }
        }

        public async Task Open()
        {
            await ModalRef.Open();
            StateHasChanged();
        }

        public async Task Close()
        {
            await ModalRef.Close();
            StateHasChanged();
        }

        private async Task HandleCancel()
        {
            await OnCancel.InvokeAsync();
            await Close();
        }
    }
}
