using JT_Classes.Financeiro;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Models
{
    /// <summary>
    /// Modelo de visualização para agrupar despesas em uma única fatura.
    /// </summary>
    public class FaturaViewModel
    {
        public DateTime DataVencimento { get; set; }
        public decimal ValorTotal => Lancamentos.Sum(l => l.Valor);
        public int QuantidadeLancamentos => Lancamentos.Count;
        public List<Despesa> Lancamentos { get; set; } = new();
        public bool EstaExpandida { get; set; } = false; // Começa recolhida por padrão
        public bool EstaPaga => Lancamentos.All(l => l.DetalhesCartaoCredito?.ParcelaPaga == true);
    }
}
