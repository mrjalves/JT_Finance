namespace JT_Finance.Shared.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    public class DespesaModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        // Propriedade auxiliar para binding com InputNumber/CustomInput type="number"
        public string ValorAsString
        {
            get => Valor.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue))
                {
                    Valor = decimalValue;
                }
                else
                {
                    // Tratar erro de conversão se necessário, ou deixar a validação do Range pegar
                }
            }
        }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; } = DateTime.Now;

        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres.")]
        public string Categoria { get; set; }
    }
}
