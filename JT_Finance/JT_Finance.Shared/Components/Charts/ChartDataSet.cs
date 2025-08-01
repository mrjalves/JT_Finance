using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Components.Charts
{
    public class ChartDataset
    {
        public string Label { get; set; } = "Dataset";
        public List<decimal> Data { get; set; } = [];

        // Propriedades para customização de cor
        public List<string>? BackgroundColor { get; set; }
        public List<string>? BorderColor { get; set; }
        public int BorderWidth { get; set; } = 1;
        public bool Fill { get; set; } // Importante para gráficos de linha (área) e radar
    }
    public enum ChartType
    {
        Bar,
        Line,
        Pie,
        Doughnut,
        Radar
    }
}
