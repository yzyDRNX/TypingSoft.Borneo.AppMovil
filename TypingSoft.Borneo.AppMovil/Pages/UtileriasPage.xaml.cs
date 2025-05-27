namespace TypingSoft.Borneo.AppMovil.Pages;
using TypingSoft.Borneo.AppMovil.VModels;

public partial class UtileriasPage : ContentPage
{
    public string FechaGuardada { get; private set; }
    public UtileriasPage()
	{
		InitializeComponent();
        var vm = new UtileriasPageViewModel();
        BindingContext = vm;
        FechaGuardada = vm.FechaActual;
    }
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;

    private async void Impresion(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(ImpresionPage));
    }
}