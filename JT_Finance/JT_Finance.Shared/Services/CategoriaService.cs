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

        public CategoriaService(ToastService toastService)
        {
            _categorias = new();
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

        public async Task<List<Categoria>> GetCategorias(string Classe, string Propriedade)
        {
            if(!_categorias.Any())
                _categorias = Categoria.ListarPorPapel(Classe, Propriedade);
            return await Task.FromResult(_categorias);
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
