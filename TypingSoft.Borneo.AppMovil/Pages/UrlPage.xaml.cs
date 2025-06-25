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
}