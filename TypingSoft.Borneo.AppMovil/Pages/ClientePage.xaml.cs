namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class ClientePage : ContentPage
{
	public ClientePage()
	{
		InitializeComponent();
	}

	private async void Surtir(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(RepartoPage));
    }
}