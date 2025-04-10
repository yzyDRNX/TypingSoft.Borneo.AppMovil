namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class RepartoPage : ContentPage
{
	public RepartoPage()
	{
		InitializeComponent();
	}

    private async void Concluir(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(UtileriasPage));
    }
}