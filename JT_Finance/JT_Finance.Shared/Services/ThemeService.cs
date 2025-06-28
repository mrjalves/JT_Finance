using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace JT_Finance.Shared.Services
{
    public class ThemeService : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;

        public string CurrentTheme { get; private set; } = "system"; // Default theme

        public event Action? OnThemeChanged;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/themeInterop.js");
            await LoadThemeFromLocalStorage();
        }

        private async Task LoadThemeFromLocalStorage()
        {
            try
            {
                if (_module == null) _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/themeInterop.js");

                var storedTheme = await _module.InvokeAsync<string?>("getLocalStorage", "theme");
                if (!string.IsNullOrEmpty(storedTheme))
                {
                    CurrentTheme = storedTheme;
                }
                else
                {
                    CurrentTheme = "system"; // Default if nothing is stored
                }
                await ApplyThemeInternal(CurrentTheme);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading theme from local storage: {ex.Message}");
                CurrentTheme = "system"; // Fallback to system default on error
                await ApplyThemeInternal(CurrentTheme);
            }
        }

        public async Task SetThemeAsync(string theme)
        {
            if (theme != "light" && theme != "dark" && theme != "system")
            {
                theme = "system"; // Default to system if an invalid theme is provided
            }

            CurrentTheme = theme;
            try
            {
                if (_module == null) _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/themeInterop.js");

                await _module.InvokeVoidAsync("setLocalStorage", "theme", CurrentTheme);
                await ApplyThemeInternal(CurrentTheme);
                OnThemeChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting theme: {ex.Message}");
            }
        }

        private async Task ApplyThemeInternal(string theme)
        {
            if (_module == null) return;

            string effectiveTheme = theme;
            if (theme == "system")
            {
                effectiveTheme = await _module.InvokeAsync<string>("getSystemPreference");
            }

            // Apply the theme class/attribute to the document body or html element
            // Example: document.documentElement.setAttribute('data-theme', effectiveTheme);
            await _module.InvokeVoidAsync("setDocumentTheme", effectiveTheme);
            Console.WriteLine($"Theme applied: {theme}, Effective: {effectiveTheme}");
        }

        public async Task ToggleThemeAsync()
        {
            string nextTheme = CurrentTheme switch
            {
                "light" => "dark",
                "dark" => "system",
                "system" => "light",
                _ => "system" // Default fallback
            };
            await SetThemeAsync(nextTheme);
        }


        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch (JSDisconnectedException) { /* Ignorar erros de desconexão */ }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing JS module: {ex.Message}");
                }
            }
        }
    }
}
