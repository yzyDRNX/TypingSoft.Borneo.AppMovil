using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace TypingSoft.Borneo.AppMovil.Pages;


public partial class EmpleadosPage : ContentPage
{
    private List<string> empleadosSeleccionados = new List<string>();

    public EmpleadosPage()
	{
		InitializeComponent();
        CargarEmpleadosFicticios();

    }

    private void CargarEmpleadosFicticios()
    {
        var empleados = new List<string>
        {
            "Seleccione un empleado",
            "Juan Pérez - Operador",
            "María López - Repartidor",
            "Carlos Gómez - Chofer",
            "Ana Martínez - Supervisora",
            "Roberto Sánchez - Cargador",
            "Daniela Torres - Operadora",
            "Alejandro Ramírez - Chofer",
            "Sofía Castro - Asistente"
        };

        foreach (var empleado in empleados)
        {
            empleadosPicker.Items.Add(empleado);
        }
    }

    private void OnAddEmployeeClicked(object sender, EventArgs e)
    {
        // Verificar si se ha seleccionado un empleado
        if (empleadosPicker.SelectedIndex == -1)
        {
            DisplayAlert("Aviso", "Por favor seleccione un empleado", "OK");
            return;
        }

        string empleadoSeleccionado = empleadosPicker.Items[empleadosPicker.SelectedIndex];

        // Verificar si el empleado ya está en la lista
        if (empleadosSeleccionados.Contains(empleadoSeleccionado))
        {
            DisplayAlert("Aviso", "Este empleado ya ha sido añadido", "OK");
            return;
        }

        // Añadir el empleado a la lista interna
        empleadosSeleccionados.Add(empleadoSeleccionado);

        // Añadir el empleado visualmente a la lista
        var empleadoFrame = new Frame
        {
            BorderColor = Color.FromArgb("#DDDDDD"),
            Padding = new Thickness(10, 5),
            Margin = new Thickness(0, 0, 0, 5),
            CornerRadius = 0
        };

        var empleadoGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        var empleadoLabel = new Label
        {
            Text = empleadoSeleccionado,
            VerticalOptions = LayoutOptions.Center
        };

        var removeButton = new Button
        {
            Text = "✕",
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#FF6347"),
            WidthRequest = 30,
            HeightRequest = 30,
            Padding = new Thickness(0),
            FontSize = 15,
            CornerRadius = 0
        };

        // Establecer la columna usando el método correcto
        Grid.SetColumn(removeButton, 1);

        // Agregar identificador para el botón de eliminar
        removeButton.BindingContext = empleadoSeleccionado;
        removeButton.Clicked += OnRemoveEmployeeClicked;

        empleadoGrid.Add(empleadoLabel, 0, 0);  // Especificar columna y fila
        empleadoGrid.Add(removeButton, 1, 0);   // Especificar columna y fila

        empleadoFrame.Content = empleadoGrid;

        empleadosSeleccionadosStack.Add(empleadoFrame);
    }

    private void OnRemoveEmployeeClicked(object sender, EventArgs e)
    {
        // Obtener el botón que fue presionado
        var removeButton = (Button)sender;

        // Obtener el nombre del empleado desde el BindingContext
        string empleado = (string)removeButton.BindingContext;

        // Eliminar de la lista interna
        empleadosSeleccionados.Remove(empleado);

        // Eliminar de la vista (eliminar el Frame padre)
        var grid = (Grid)removeButton.Parent;
        var frame = (Frame)grid.Parent;
        empleadosSeleccionadosStack.Remove(frame);
    }

    private async void OnStartRouteClicked(object sender, EventArgs e)
    {
        // Verificar si hay empleados seleccionados
        if (empleadosSeleccionados.Count == 0)
        {
            await DisplayAlert("Aviso", "Debe seleccionar al menos un empleado para iniciar la ruta", "OK");
            return;
        }

        // Mostrar mensaje de inicio de ruta (representativo)
        await DisplayAlert("Ruta Iniciada",
                         $"Iniciando ruta con {empleadosSeleccionados.Count} empleados seleccionados",
                         "OK");

        // Aquí normalmente se navegaría a la siguiente pantalla
        // await Navigation.PushAsync(new RutaPage());
        // Usar CustomNavigation para navegar a ClientePage
        await App.NavigationService.Navegar(nameof(MenuPage));
    }
}