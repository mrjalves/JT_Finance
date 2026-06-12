using JT_Aplicacao;

using JT_Atributos;

using JT_Classes.Comum;

using System.ComponentModel.DataAnnotations;

namespace JT_Finance.Shared.Models
{
    [NomeTabela("financeiro_configuracoes")]
    public class MConfiguracao : Configuracao
    {
        public MConfiguracao() : base()
        {
        }

        public MConfiguracao(int id)
        {
        }

        #region CONFIGURAÇÕES FINANCEIRAS

        // Aqui podes colocar as tuas propriedades específicas do financeiro.
        // Estes são apenas exemplos, ajusta conforme as tuas necessidades.

        [NomeColuna("MoedaPadrao")]
        public string MoedaPadrao { get; set; } = "BRL";

        [NomeColuna("AlertaSaldoNegativo")]
        public bool AlertaSaldoNegativo { get; set; } = true;

        [NomeColuna("DiaFechamentoFatura")]
        public int DiaFechamentoFatura { get; set; } = 5;

        #endregion

        // Sobrescrita obrigatória da tua classe base
        public override List<Filtro> CarregarFiltros() => [];
    }
}