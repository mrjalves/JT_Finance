using JT_Classes.Financeiro;
using JT_Classes.structs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Repository
{
    public class DesktopDespRep : IDespesaRep
    {
        public Task<List<sDespesa>> GetAllAsync()
        {
            return Task.FromResult(JT_Servicos.DespesaService.GetAll());
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(JT_Servicos.DespesaService.Delete(id));
        }

        public Task<bool> DuplicarAsync(Despesa despesa)
        {
            return Task.FromResult(despesa.Duplicar());
        }

        public Task<Despesa> GetByIdAsync(int id)
        {
            // A lógica original buscava tudo e filtrava na memória. 
            // Podemos manter ou otimizar para buscar direto pelo ID se o serviço permitir.
            var despesa = JT_Servicos.DespesaService.GetById(id); // Supondo que exista um GetById
            return Task.FromResult(despesa);
        }

        public Task<bool> SaveAsync(Despesa despesa)
        {
            return Task.FromResult(JT_Servicos.DespesaService.Save(despesa));
        }

        public Task<List<sDespesa>> FindAsync(string condicao)
        {
            return Task.FromResult(JT_Servicos.DespesaService.Find(condicao));
        }

        // Implementar os outros métodos da interface da mesma forma
        public Task<List<TotalPorTipo>> GetDespesasAgrupadasPorTipoAsync(DateTime mes)
        {
            return Task.FromResult(Despesa.GetDespesasAgrupadasPorTipo(mes));
        }

        public Task<List<TotalPorCategoria>> GetDespesasAgrupadasPorCategoriaAsync(DateTime mes)
        {
            return Task.FromResult(Despesa.GetDespesasAgrupadasPorCategoria(mes));
        }

        public Task<decimal> GetTotalDespesaMesAsync(DateTime mes)
        {
            return Task.FromResult(Despesa.GetTotalDespesaMes(mes));
        }

        public Task<List<sDespesa>> GetDespesasProximasAoVencimentoAsync()
        {
            return Task.FromResult(Despesa.GetDespesasProximasAoVencimento());
        }

        public Task<List<sDespesa>> GetDespesasVencidasAsync()
        {
            return Task.FromResult(Despesa.GetDespesasVencidas());
        }

        public Task<List<sDespesa>> GetDespesasPorMesAsync(DateTime mesReferencia)
        {
            return Task.FromResult(Despesa.GetDespesasPorMes(mesReferencia));
        }

     
    }
}
