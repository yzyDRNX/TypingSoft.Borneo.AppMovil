using TypingSoft.Borneo.AppMovil.VModels;

namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class InicioPage : ContentPage
{
    private UrlPageVM ViewModel => BindingContext as UrlPageVM;

    public InicioPage()
    {
        InitializeComponent();
        BindingContext = new UrlPageVM();
    }

    private async void OnSendUrlClicked(object sender, EventArgs e)
    {
        ViewModel?.GuardarUrlCommand.Execute(null);
        await App.NavigationService.Navegar(nameof(LoginPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        MainContentStack.Opacity = 0;
        MainContentStack.Scale = 0.97;
        await MainContentStack.FadeTo(1, 400, Easing.CubicInOut);
        await MainContentStack.ScaleTo(1, 300, Easing.CubicOut);
    }
}