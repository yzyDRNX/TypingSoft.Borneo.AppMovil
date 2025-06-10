namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
	}
	private async void OnSendUrlClicked(object sender, EventArgs e)
    {
        await App.NavigationService.Navegar(nameof(LoginPage));
    }
}