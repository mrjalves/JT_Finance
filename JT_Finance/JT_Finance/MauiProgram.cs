

using JT_Finance.Services;
using JT_Finance.Shared.Layout;
using JT_Finance.Shared.Repository;
using JT_Finance.Shared.Services;
using JT_Finance.Shared.Services.Interfaces;

using JT_UserInterface.Shared.Models;
using JT_UserInterface.Shared.Services;
using JT_UserInterface.Shared.Services.Interfaces;

using Microsoft.Extensions.Logging;

namespace JT_Finance
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddScoped<IDespesaRep, DesktopDespRep>();
            builder.Services.AddScoped<ICartaoCreditoRep, DesktopCartaoCreditoRep>();
            builder.Services.AddScoped<IReceitaRep, DesktopReceitaRep>();
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<ToastService>();
            builder.Services.AddSingleton<FiltroService>();
            builder.Services.AddScoped<ICartaoCreditoService, CartaoCreditoService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<IReceitaService, ReceitaService>();
            builder.Services.AddScoped<IDespesaService, DespesaService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<IThemeService, ThemeService>();
            builder.Services.AddSingleton<CurrentUserState>();
            builder.Services.AddScoped<FirebaseAuthService>();
            builder.Services.AddScoped<IMenuProvider, FinanceMenuProvider>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
