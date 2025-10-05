using JT_Classes.Financeiro;

using JT_Servicos;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class DesktopReceitaRep : IReceitaRep
    {
        public Task<List<Receita>> GetAllAsync()
        {
            return Task.FromResult(ReceitaService.GetAll());
        }

        public Task<Receita> GetByIdAsync(int id)
        {
            // Assumindo que seu serviço estático tenha um método GetById
            return Task.FromResult(ReceitaService.GetById(id));
        }

        public Task<bool> SaveAsync(Receita receita)
        {
            return Task.FromResult(ReceitaService.Save(receita));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(ReceitaService.Delete(id));
        }

        public Task<bool> DuplicateAsync(int id)
        {
            var receita = ReceitaService.GetById(id);
            if (receita == null) return Task.FromResult(false);

            return Task.FromResult(receita.Duplicar());
        }

        public Task<decimal> GetTotalMesAsync(DateTime mes)
        {
            return Task.FromResult(Receita.GetTotalReceitasMes(mes));
        }

        public Task<List<Receita>> FindAsync(string condicao)
        {
            return Task.FromResult(ReceitaService.Find(condicao));
        }
    }
}