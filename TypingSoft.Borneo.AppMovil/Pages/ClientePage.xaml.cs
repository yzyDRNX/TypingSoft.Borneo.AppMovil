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

    private void OnAñadirClienteClicked(object sender, EventArgs c)
    {
        // Obtener el cliente seleccionado del Picker
        var clienteSeleccionado = clientesPicker.SelectedItem as Models.Custom.ClientesLista;

        if (clienteSeleccionado != null)
        {
            // Verificar si el cliente ya está en la lista
            if (clientesSeleccionados.Contains(clienteSeleccionado.IdClienteAsociado))
            {
                // Mostrar un mensaje de alerta si el cliente ya está en la lista
                DisplayAlert("Advertencia", "El cliente ya está en la lista.", "OK");
                return;
            }

            // Añadir el cliente al HashSet
            clientesSeleccionados.Add(clienteSeleccionado.IdClienteAsociado);

            // Crear un nuevo Label para mostrar el nombre del cliente seleccionado
            var clienteLabel = new Label
            {
                Text = clienteSeleccionado.Cliente, // Accede directamente a la propiedad 'Cliente'
                FontSize = 14,
                TextColor = Colors.White,
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Añadir el Label al StackLayout
            clientesSeleccionadosStack.Children.Add(clienteLabel);
        }
        else
        {
            // Mostrar un mensaje si no se seleccionó ningún empleado
            DisplayAlert("Advertencia", "Por favor, selecciona un empleado antes de añadirlo.", "OK");
        }
    }

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


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
            await ViewModel.ObtenerClientesAsync();
    }


}