using JT_Classes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> GetCategorias();
        Task<List<Categoria>> GetCategoriasDespesa();
        Task<List<Categoria>> GetTipoCategoriasDespesa();
        Task<List<Categoria>> GetTipoCategoriasReceitas();
        Task<List<Categoria>> GetCategoriasReceitas();
        Task<Categoria> CriarCategoriaAsync(Categoria categoria);
        Task<bool> SalvarCategoriaAsync(Categoria categoria);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;
    }
}
