namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class MenuPage : ContentPage
{
	public MenuPage()
	{
		InitializeComponent();
	}

    private async void OnGenerarVentaClicked(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(ClientePage));
    }
}