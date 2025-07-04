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
        private string? _primerEmpleadoSeleccionado; // Guarda el primer empleado

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
                    _primerEmpleadoSeleccionado = null; // Reinicia al cargar lista nueva
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

            // Solo guarda el primer empleado seleccionado en StaticSettings
            if (_primerEmpleadoSeleccionado == null)
            {
                _primerEmpleadoSeleccionado = empleadoSeleccionado.Empleado ?? string.Empty;
                Helpers.StaticSettings.FijarConfiguracion("Empleado", _primerEmpleadoSeleccionado);
                // Ya NO insertes aquí un TicketDetalleLocal vacío
            }
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
                IdRuta = ViewModel.IdRutaActual,
                Fecha = DateTime.Now,
                Vuelta = 1,
                IdStatusVenta = Guid.NewGuid(),
                Sincronizado = false // <-- ¡Esto es clave!
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