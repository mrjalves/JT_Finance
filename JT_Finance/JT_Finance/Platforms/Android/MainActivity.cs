using Android.App;
using Android.Content; // Necessário para o IntentFilter
using Android.Content.PM;
using Android.OS;

namespace JT_Finance
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "jt_aplicacao"
    )]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.googleusercontent.apps.521428017393-g4lhesrtp79b33vanf8r2dj0k9mrqebf"
    )]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.googleusercontent.apps.521428017393-4sk2mf0i6v2lprgvhn0o6oi2qil0m66m"
    )]
    public class WebAuthenticatorCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
    {
    }
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}