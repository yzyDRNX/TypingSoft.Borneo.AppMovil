namespace TypingSoft.Borneo.AppMovil.Pages;
using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API; // Asegúrate de que esta referencia sea correcta
using TypingSoft.Borneo.AppMovil.VModels;


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
    private async void OnEmpeladoClicked(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(EmpleadosPage));
    }
}