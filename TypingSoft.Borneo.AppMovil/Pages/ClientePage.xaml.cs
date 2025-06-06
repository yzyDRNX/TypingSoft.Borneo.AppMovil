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
    private readonly HashSet<Guid> clientesSeleccionados = new HashSet<Guid>();

    private async void SurtirClicked(object sender, EventArgs e)
    {
        // Navegacion a RepartoPage
        await Navigation.PushAsync(new RepartoPage());
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

        if (vm.ClientesASurtir.Count > 0)
        {
            await DisplayAlert("Aviso", "Solo puedes añadir un cliente a la vez.", "OK");
            return;
        }

        if (vm.ClientesASurtir.Contains(clienteSeleccionado))
        {
            await DisplayAlert("Aviso", "Este cliente ya fue añadido.", "OK");
            return;
        }

        // ✅ Invoca el método Surtir del ViewModel
        await vm.Surtir(clienteSeleccionado);
    }


    private async void OnRepartoClicked(object sender, EventArgs e)
    {
        // Verifica si hay clientes seleccionados
        var vm = BindingContext as TypingSoft.Borneo.AppMovil.VModels.CatalogosVM;
        if (vm.ClientesASurtir.Count == 0)
        {
            await DisplayAlert("Advertencia", "Debes seleccionar al menos un cliente antes de continuar.", "OK");
            return;
        }

        // Si hay al menos un cliente, procede a la siguiente página
        await Navigation.PushAsync(new RepartoPage());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            await ViewModel.ObtenerClientesAsync();
    }


}