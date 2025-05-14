using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class EmpleadosPage : ContentPage
{
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;

    public EmpleadosPage()
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
    private readonly HashSet<Guid> empleadosSeleccionados = new HashSet<Guid>();

    private void OnAñadirEmpleadoClicked(object sender, EventArgs e)
    {
        // Obtener el empleado seleccionado del Picker
        var empleadoSeleccionado = empleadosPicker.SelectedItem as Models.Custom.EmpleadosLista;

        if (empleadoSeleccionado != null)
        {
            // Verificar si el empleado ya está en la lista
            if (empleadosSeleccionados.Contains(empleadoSeleccionado.Id))
            {
                // Mostrar un mensaje de alerta si el empleado ya está en la lista
                DisplayAlert("Advertencia", "El empleado ya está en la lista.", "OK");
                return;
            }

            // Añadir el empleado al HashSet
            empleadosSeleccionados.Add(empleadoSeleccionado.Id);

            // Crear un nuevo Label para mostrar el nombre del empleado seleccionado
            var empleadoLabel = new Label
            {
                Text = empleadoSeleccionado.Empleado, // Accede directamente a la propiedad 'Empleado'
                FontSize = 14,
                TextColor = Colors.White,
                Margin = new Thickness(0, 5, 0, 0)
            };

            // Añadir el Label al StackLayout
            empleadosSeleccionadosStack.Children.Add(empleadoLabel);
        }
        else
        {
            // Mostrar un mensaje si no se seleccionó ningún empleado
            DisplayAlert("Advertencia", "Por favor, selecciona un empleado antes de añadirlo.", "OK");
        }
    }

    private async void OnEmpezarRutaClicked(object sender, EventArgs e)
    {
        //Navegacion a ClientePage
        await Navigation.PushAsync(new ClientePage());
    }



    private void CrearViewModelManualmente()
    {
        var catalogosService = new Services.CatalogosService();
        var catalogosBL = new BL.CatalogosBL(catalogosService);
        this.BindingContext = new VModels.CatalogosVM(catalogosBL);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Cargar los empleados cuando la página aparece
        ViewModel?.ObtenerEmpleados();
    }
}
