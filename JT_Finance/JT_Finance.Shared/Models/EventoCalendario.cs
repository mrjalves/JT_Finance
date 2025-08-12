using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Models
{
    public enum TipoEvento
    {
        Receita,
        Despesa
    }

    // Classe unificada para exibição no calendário
    public class EventoCalendario
    {
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TipoEvento Tipo { get; set; }
    }
}
