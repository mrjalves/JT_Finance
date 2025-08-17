using JT_Finance.Shared.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services.Interfaces
{
    public interface IThemeService
    {
        event Action OnChange;
        Theme CurrentTheme { get; }
        Task SetThemeAsync(Theme theme);
        List<Theme> GetAvailableThemes();
    }
}
