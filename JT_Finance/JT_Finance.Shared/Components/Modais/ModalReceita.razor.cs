using JT_Aplicacao;

using JT_Classes;
using JT_Classes.Financeiro;
using JT_Classes.Comum;
using JT_Classes.structs;

using JT_Servicos;
using JT_Servicos.Financeiro;

using JT_UserInterface.Shared.Components;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Text;
using JT_UserInterface.Shared.Components.Core;
using JT_Aplicacao.Classes;

namespace JT_Finance.Shared.Components.Modais
{
    public partial class ModalReceita : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public Receita? Receita { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private List<sCategoria> TiposReceita = new();
        private List<sCategoria> CategoriasReceita = new();
        private bool showNewTipoForm = false;
        private bool showNewCategoriaForm = false;
        private string novaCategoriaDescricao = string.Empty;
        private Modal ModalRef = new();

        protected override async Task OnInitializedAsync()
        {
            await CarregarOpcoes();
        }

        public void Dispose()
        {
        }

        private async void AtualizarOpcoes()
        {
            await CarregarOpcoes();
            StateHasChanged();
        }

        private async Task CarregarOpcoes()
        {
            try
            {
                TiposReceita = await CategoriaService.ListarPorPapelAsync("Receita", "TipoReceita");
                CategoriasReceita = await CategoriaService.ListarPorPapelAsync("Receita", "Categoria");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar opções: {ex.Message}");
                TiposReceita = new List<sCategoria>();
                CategoriasReceita = new List<sCategoria>();
            }
        }

        private async Task HandleSubmit()
        {
            if (Receita != null)
            {
                bool sucesso = await ReceitaService.SalvarReceitaAsync(Receita);
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

        private void ToggleInlineForm(string tipo)
        {
            novaCategoriaDescricao = string.Empty;
            if (tipo == "TipoReceita")
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
                TipoEntidade = "Receita",
                Nome = tipo,
                Descricao = novaCategoriaDescricao,
                Status = StatusObjeto.Novo
            };

            var categoriaSalva = await CategoriaService.SalvarCategoriaAsync(novaCategoria);
            if (categoriaSalva != null)
            {
                if (tipo == "TipoReceita")
                {
                    Receita.TipoReceita = categoriaSalva;
                    showNewTipoForm = false;
                }
                else
                {
                    Receita.Categoria = categoriaSalva;
                    showNewCategoriaForm = false;
                }
                novaCategoriaDescricao = string.Empty;
            }
        }
    }
}


