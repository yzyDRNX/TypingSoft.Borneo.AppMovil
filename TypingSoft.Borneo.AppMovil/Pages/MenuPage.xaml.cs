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

        // Animación de fade-in y escala para el layout principal
        MainMenuLayout.Opacity = 0;
        MainMenuLayout.Scale = 0.97;
        await MainMenuLayout.FadeTo(1, 400, Easing.CubicInOut);
        await MainMenuLayout.ScaleTo(1, 300, Easing.CubicOut);

        // Animación secuencial para los frames de los botones
        var frames = new[] { FrameDescargarDatos, FrameSincronizarDatos, FrameIniciar, FrameCerrarSesion };
        foreach (var frame in frames)
        {
            frame.Opacity = 0;
            frame.TranslationY = 30;
        }
        int delay = 0;
        foreach (var frame in frames)
        {
            await Task.Delay(delay);
            await Task.WhenAll(
                frame.FadeTo(1, 350, Easing.CubicIn),
                frame.TranslateTo(0, 0, 350, Easing.CubicOut)
            );
            delay += 80; // efecto cascada
        }
    }
}