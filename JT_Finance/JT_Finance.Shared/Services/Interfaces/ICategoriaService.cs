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
        Task<List<Categoria>> GetCategorias(string Classe, string Propriedade);
        Task<Categoria> CriarCategoriaAsync(Categoria categoria);
        Task<bool> SalvarCategoriaAsync(Categoria categoria);
        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        event Action OnDataChanged;
    }
}
