namespace TypingSoft.Borneo.AppMovil.Pages;
using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API; // Asegúrate de que esta referencia sea correcta
using TypingSoft.Borneo.AppMovil.VModels;


public partial class MenuPage : ContentPage
{
     public MenuPage()
	{
		InitializeComponent();
        var viewModel = App.ServiceProvider.GetService<VModels.CatalogosVM>();
        if (viewModel != null)
            this.BindingContext = viewModel;
    }
    private async void OnGenerarVentaClicked(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(ClientePage));
    }
    private async void OnEmpeladoClicked(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(EmpleadosPage));
    }
}