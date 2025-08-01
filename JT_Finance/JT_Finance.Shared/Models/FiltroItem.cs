using System.Collections.Generic;

namespace JT_Finance.Shared.Models
{
    public class FiltroItem
    {
        public string Label { get; set; }
        public string ColumnName { get; set; }
        public object Value { get; set; }
        public string Type { get; set; } = "text";
        public List<KeyValuePair<string, string>> Options { get; set; } = new();
    }
}
