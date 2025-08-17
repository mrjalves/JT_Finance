using JT_Finance.Shared.Models;
using JT_Finance.Shared.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class ThemeService : IThemeService
    {
        public event Action OnChange;
        public Theme CurrentTheme { get; private set; }

        private readonly List<Theme> _availableThemes;

        public ThemeService()
        {
            _availableThemes = new List<Theme>
            {
                new Theme
                {
                    Name = "Light",
                    Colors = new Dictionary<string, string>
                    {
                        { "--primary-color", "#007bff" },
                        { "--background-color", "#ffffff" },
                        { "--text-color", "#333333" }
                    }
                },
                new Theme
                {
                    Name = "Dark",
                    Colors = new Dictionary<string, string>
                    {
                        { "--primary-color", "#1e90ff" },
                        { "--background-color", "#2c2c2c" },
                        { "--text-color", "#ffffff" }
                    }
                }
            };
            CurrentTheme = _availableThemes[0];
        }

        public List<Theme> GetAvailableThemes()
        {
            return _availableThemes;
        }

        public Task SetThemeAsync(Theme theme)
        {
            CurrentTheme = theme;
            NotifyStateChanged();
            return Task.CompletedTask;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
