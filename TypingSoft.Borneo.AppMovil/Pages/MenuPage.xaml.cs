using Microsoft.Maui.ApplicationModel;
using TypingSoft.Borneo.AppMovil.Helpers;

namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
        var viewModel = App.ServiceProvider.GetService<VModels.MenuVM>();
        if (viewModel != null)
            this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Aplicar tema guardado (si existe)
        var savedTheme = Settings.ObtenerValor<string>("AppTheme");
        if (!string.IsNullOrWhiteSpace(savedTheme))
            ApplyTheme(savedTheme);

        // Sin animaciones: asegurar estado final directamente
        MainMenuLayout.Opacity = 1;
        MainMenuLayout.Scale = 1;

        var frames = new[] { FrameDescargarDatos, FrameSincronizarDatos, FrameIniciar, FrameCerrarSesion };
        foreach (var frame in frames)
        {
            frame.Opacity = 1;
            frame.TranslationY = 0;
        }

        // Solicitar permiso de dispositivos cercanos (Bluetooth)
        var nearbyStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
    }

    private void ApplyTheme(string theme)
    {
        if (Enum.TryParse<AppTheme>(theme, out var parsed))
            Application.Current!.UserAppTheme = parsed;
    }

}