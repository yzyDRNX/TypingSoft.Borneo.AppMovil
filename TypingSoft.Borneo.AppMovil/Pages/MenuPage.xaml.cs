namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class MenuPage : ContentPage
{
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
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