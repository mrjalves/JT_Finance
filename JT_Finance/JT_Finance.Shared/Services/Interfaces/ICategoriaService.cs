using JT_Classes;
using JT_Classes.structs;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<sCategoria>> ObterTodasCategoriasAsync();
        Task<Categoria?> ObterCategoriaPorId(int id);
        Task<int> SalvarCategoriaAsync(Categoria categoria);
        Task<bool> RemoverCategoriaAsync(int id);

        // Método específico de Categoria para buscar os papéis (ex: TipoDespesa)
        Task<List<sCategoria>> ListarPorPapelAsync(string tipoEntidade, string papelCategoria);

        void NotificarErro(string titulo, string mensagem);
        void NotificarSucesso(string titulo, string mensagem);
        Task CarregarDadosAsync();

        event Action OnDataChanged;
    }
}