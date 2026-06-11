using JT_Classes;
using JT_Classes.structs;

using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ToastService _toastService;
        private List<sCategoria> _scategorias;
        public event Action? OnDataChanged;

        // Injeção do serviço base que acessa a API (JT_Servicos.CategoriaService)
        public JT_Servicos.CategoriaService categoriaServiceBase { get; set; }

        public CategoriaService(ToastService toastService, JT_Servicos.CategoriaService categoriaServiceBase)
        {
            _toastService = toastService;
            _scategorias = new List<sCategoria>();
            this.categoriaServiceBase = categoriaServiceBase;
        }

        public async Task CarregarDadosAsync()
        {
            // Busca os dados da API e converte as entidades completas para a struct sCategoria
            var categorias = await categoriaServiceBase.GetAll();
            _scategorias = categorias.Select(c => sCategoria.FromEntity(c)).ToList();
        }

        public async Task<List<sCategoria>> ObterTodasCategoriasAsync()
        {
            if (!_scategorias.Any())
            {
                await CarregarDadosAsync();
            }
            return _scategorias;
        }

        public async Task<Categoria?> ObterCategoriaPorId(int id)
        {
            return await categoriaServiceBase.GetById(id);
        }

        public async Task<int> SalvarCategoriaAsync(Categoria categoria)
        {
            var sucesso = await categoriaServiceBase.SaveAsync(categoria);
            var sCat = sCategoria.Empty;
            if (sucesso > 0)
            {
                NotificarSucesso("Sucesso", "Categoria salva com sucesso!");

                // Recarrega o objeto pelo ID recém-salvo para garantir que a Struct está espelhando o BD
                var categoriaSalva = await categoriaServiceBase.GetById(sucesso);
                if (categoriaSalva != null)
                {
                     sCat = sCategoria.FromEntity(categoriaSalva);

                    // Como sCategoria é uma "record struct", usamos FindIndex que é mais seguro do que comparar com null
                    var index = _scategorias.FindIndex(c => c.Id == sucesso);
                    if (index >= 0)
                    {
                        _scategorias[index] = sCat;
                    }
                    else
                    {
                        _scategorias.Add(sCat);
                    }
                }
                NotifyDataChanged();
            }
            else
            {
                NotificarErro("Erro", "Falha ao salvar a categoria.");
            }

            return sucesso;
        }

        public async Task<bool> RemoverCategoriaAsync(int id)
        {
            bool sucesso = await categoriaServiceBase.Delete(id);

            if (sucesso)
            {
                NotificarSucesso("Sucesso", "Categoria removida com sucesso!");

                // Remove do cache em vez de dar full reload
                var index = _scategorias.FindIndex(c => c.Id == id);
                if (index >= 0)
                {
                    _scategorias.RemoveAt(index);
                }

                NotifyDataChanged();
            }
            else
            {
                NotificarErro("Erro", "Falha ao remover a categoria.");
            }

            return sucesso;
        }

        public async Task<List<sCategoria>> ListarPorPapelAsync(string tipoEntidade, string papelCategoria)
        {
            // Chama a lógica de papel do JT_Servicos e converte em struct pro componente visual
            var categorias = await categoriaServiceBase.ListarPorPapelAsync(tipoEntidade, papelCategoria);
            return categorias.Select(c => sCategoria.FromEntity(c)).ToList();
        }

        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }

        private void NotifyDataChanged() => OnDataChanged?.Invoke();
    }
}