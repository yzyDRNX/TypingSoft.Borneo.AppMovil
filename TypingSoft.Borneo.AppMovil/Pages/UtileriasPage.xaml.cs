namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class UtileriasPage : ContentPage
{
	public UtileriasPage()
	{
		InitializeComponent();
	}
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
    private async void Impresion(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(ImpresionPage));
    }
}