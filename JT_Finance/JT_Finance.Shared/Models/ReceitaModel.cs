using System.ComponentModel.DataAnnotations;

namespace JT_Finance.Shared.Models;

public class ReceitaModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres.")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    public string ValorAsString
    {
        get => Valor.ToString(System.Globalization.CultureInfo.InvariantCulture);
        set
        {
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var decimalValue))
            {
                Valor = decimalValue;
            }
        }
    }

    [Required(ErrorMessage = "A data é obrigatória.")]
    public DateTime Data { get; set; } = DateTime.Now;

    [StringLength(50, ErrorMessage = "A fonte deve ter no máximo 50 caracteres.")]
    public string Fonte { get; set; }
}
