using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class ClientePage : ContentPage
{

    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
    public ClientePage()
	{
        InitializeComponent();

        // Obtener el ViewModel del contenedor de servicios
        if (App.ServiceProvider != null)
        {
            var viewModel = App.ServiceProvider.GetService<VModels.CatalogosVM>();
            if (viewModel != null)
            {
                this.BindingContext = viewModel;
            }
            else
            {
                // Si no se puede obtener del contenedor, crear manualmente
                CrearViewModelManualmente();
            }
        }
        else
        {

            CrearViewModelManualmente();
        }
    }

	private async void Surtir(object sender, EventArgs e)
    {
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(RepartoPage));
    }

    private void CrearViewModelManualmente()
    {
        var catalogosService = new Services.CatalogosService();
        var catalogosBL = new BL.CatalogosBL(catalogosService);

        // Creamos también la instancia de la BD local
        var localDb = new Services.LocalDatabaseService();

        // Le pasamos ambos al VM
        this.BindingContext = new VModels.CatalogosVM(catalogosBL, localDb);
    }

    private async void OnAñadirClienteClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as TypingSoft.Borneo.AppMovil.VModels.CatalogosVM;
        var clienteSeleccionado = clientesPicker.SelectedItem as TypingSoft.Borneo.AppMovil.Models.Custom.ClientesLista;

        if (clienteSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Por favor selecciona un cliente.", "OK");
            return;
        }

        if (vm != null && vm.ClientesASurtir.Contains(clienteSeleccionado))
        {
            await DisplayAlert("Aviso", "Este cliente ya fue añadido.", "OK");
            return;
        }

        if (vm != null)
        {
            vm.ClientesASurtir.Add(clienteSeleccionado);
        }
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            await ViewModel.ObtenerClientesAsync();
    }


}