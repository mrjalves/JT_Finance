// Namespace should match the project structure.
// Assuming JT_Finance.Shared.Models based on typical conventions.
namespace JT_Finance.Shared.Models
{
    // Required for EventCallback
    using Microsoft.AspNetCore.Components;

    public class ToolStripItem
    {
        public string? Text { get; set; }
        public string? IconCssClass { get; set; }
        public EventCallback OnClick { get; set; }
        public bool IsSeparator { get; set; }
    }
}
