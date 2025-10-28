using Android.App;
using Android.Content.PM;
using Android.OS;

namespace TypingSoft.Borneo.AppMovil
{
    [Activity(
         Label = "Borneo", // 👈 Nombre visible de la app
         Theme = "@style/Maui.SplashTheme",
         MainLauncher = true,
         LaunchMode = LaunchMode.SingleTop,
         ConfigurationChanges =
             ConfigChanges.ScreenSize |
             ConfigChanges.Orientation |
             ConfigChanges.UiMode |
             ConfigChanges.ScreenLayout |
             ConfigChanges.SmallestScreenSize |
             ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
