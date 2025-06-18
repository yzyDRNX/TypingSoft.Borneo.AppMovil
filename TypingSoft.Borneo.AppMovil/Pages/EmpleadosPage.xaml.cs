using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class EmpleadosPage : ContentPage
    {
        VModels.EmpleadosVM? ViewModel;
        private readonly HashSet<Guid> _empleadosSeleccionados = new HashSet<Guid>();

        public EmpleadosPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.EmpleadosVM>();
            if (ViewModel != null)
            {
                this.BindingContext = ViewModel;
              
            }

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
             
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ListadoEmpleados":
                    empleadosPicker.ItemsSource = ViewModel?.ListadoEmpleados;
                    empleadosPicker.SelectedItem = null; // Resetear selección al cargar la lista
                    emptyStateLabel.IsVisible = ViewModel?.ListadoEmpleados.Count == 0;
                    empleadosSeleccionadosStack.Children.Clear(); // Limpiar la lista de empleados seleccionados
                    _empleadosSeleccionados.Clear(); // Limpiar el conjunto de IDs seleccionados
                    break;  
                default:
                    break;
            }
        }

        private async void OnAñadirEmpleadoClicked(object sender, EventArgs e)
        {
            var empleadoSeleccionado = empleadosPicker.SelectedItem as Models.Custom.EmpleadosLista;

            if (empleadoSeleccionado == null)
            {
                await DisplayAlert("Advertencia", "Por favor, seleccione un empleado antes de añadirlo.", "OK");
                return;
            }

            if (_empleadosSeleccionados.Contains(empleadoSeleccionado.Id))
            {
                await DisplayAlert("Advertencia", "Este empleado ya está en la lista.", "OK");
                return;
            }

            _empleadosSeleccionados.Add(empleadoSeleccionado.Id);
            emptyStateLabel.IsVisible = false;

            var empleadoItem = new HorizontalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(0, 5)
            };

            empleadoItem.Children.Add(new Label
            {
                Text = "•",
                TextColor = Color.FromArgb("#2160AB"),
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            });

            empleadoItem.Children.Add(new Label
            {
                Text = empleadoSeleccionado.Empleado,
                TextColor = Colors.Black,
                FontSize = 14,
                VerticalOptions = LayoutOptions.Center
            });

            empleadosSeleccionadosStack.Children.Add(empleadoItem);

            // --- GUARDAR NOMBRE DEL EMPLEADO EN TICKETLOCAL ---
            var ticket = new TicketLocal
            {
                Id = Guid.NewGuid(),
                Empleado = empleadoSeleccionado.Empleado ?? string.Empty, // Guarda el nombre
                Fecha = DateTime.Now,
                Cliente = string.Empty,
                Cantidad = 0,
                Descripcion = string.Empty,
                ImporteTotal = 0m
            };

            await ViewModel._localDb.InsertarTicketAsync(ticket);

            await DisplayAlert("Éxito", "Empleado guardado en el ticket.", "OK");
        }

        private async void OnEmpezarRutaClicked(object sender, EventArgs e)
        {
            if (_empleadosSeleccionados.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar al menos un empleado para continuar.", "OK");
                return;
            }

            if (ViewModel == null || ViewModel.IdRutaActual == null)
            {
                await DisplayAlert("Error", "No se encontró la ruta actual. Asegúrate de que esté cargada.", "OK");
                return;
            }

            var nuevaVenta = new VentaGeneralLocal
            {
                IdVentaGeneral = Guid.NewGuid(),
                IdRuta = ViewModel.IdRutaActual, // ← usa .Value porque es Guid?
                Fecha = DateTime.Now,
                Vuelta = 1
            };

            await ViewModel._localDb.GuardarVentaAsync(nuevaVenta);

            var ventas = await ViewModel._localDb.ObtenerVentasAsync();
            System.Diagnostics.Debug.WriteLine($"Ventas totales registradas: {ventas.Count}");

            await Navigation.PushAsync(new ClientePage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel != null)
            {
                await ViewModel.CargarEmpleadosDesdeLocal();
            }

            // Resetear selección al aparecer
            empleadosPicker.SelectedItem = null;
        }
    }
}