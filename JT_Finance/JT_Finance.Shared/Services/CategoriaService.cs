using JT_Classes;
using JT_Classes.Financeiro;

using JT_Finance.Shared.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ToastService _toastService;
        private List<Categoria> _categorias;
        private List<Categoria> _categoriasDespesas;
        private List<Categoria> _categoriasTipoDespesas;
        private List<Categoria> _categoriasReceitas;
        private List<Categoria> _categoriasTipoReceitas;

        public CategoriaService(ToastService toastService)
        {
            _categorias = new();
            _categoriasDespesas = new();
            _categoriasReceitas = new();
            _categoriasTipoDespesas = new();
            _categoriasTipoReceitas = new();
            _toastService = toastService;
        }

        public event Action OnDataChanged;

        public async Task<Categoria> CriarCategoriaAsync(Categoria categoria)
        {
            categoria.Status = JT_Persistencia.Persistencia.StatusObjeto.Novo;
            bool sucesso = categoria.Salvar();
            if (sucesso)
            {
                await CarregarCategorias();
                return categoria;
            }
            return null;
        }

        public Task<List<Categoria>> GetCategorias()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Categoria>> GetCategoriasDespesa()
        {
            if (!_categorias.Any())
                _categoriasDespesas = Categoria.ListarPorPapel("Despesa", "Categoria");
            return await Task.FromResult(_categoriasDespesas);
        }

        public async Task<List<Categoria>> GetCategoriasReceitas()
        {
            if (!_categorias.Any())
                _categoriasReceitas = Categoria.ListarPorPapel("Receita", "CategoriaReceita");
            return await Task.FromResult(_categoriasReceitas);
        }

        public async Task<List<Categoria>> GetTipoCategoriasDespesa()
        {
            if (!_categorias.Any())
                _categoriasTipoDespesas = Categoria.ListarPorPapel("Despesa", "TipoDespesa");
            return await Task.FromResult(_categoriasTipoDespesas);
        }

        public async Task<List<Categoria>> GetTipoCategoriasReceitas()
        {
            if (!_categorias.Any())
                _categoriasTipoReceitas = Categoria.ListarPorPapel("Receita", "TipoReceita");
            return await Task.FromResult(_categoriasTipoReceitas);
        }

        public void NotificarErro(string titulo, string mensagem)
        {
            _toastService.ShowError(titulo, mensagem);
        }

        public void NotificarSucesso(string titulo, string mensagem)
        {
            _toastService.ShowSuccess(titulo, mensagem);
        }

        public async Task<bool> SalvarCategoriaAsync(Categoria categoria)
        {
            bool sucesso = categoria.Salvar();
            if (sucesso)
            {
                await CarregarCategorias();
            }
            return sucesso;
        }

        private async Task CarregarCategorias()
        {
            _categorias = new Categoria().Consultar<Categoria>("SELECT * FROM CATEGORIAS");
            OnDataChanged?.Invoke();
            await Task.CompletedTask;
        }
    }
}
