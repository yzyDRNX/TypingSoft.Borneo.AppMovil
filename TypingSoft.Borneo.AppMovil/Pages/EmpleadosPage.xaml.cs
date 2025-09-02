using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Pages.Modals;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class EmpleadosPage : ContentPage
    {
        VModels.EmpleadosVM? ViewModel;
        private readonly HashSet<Guid> _empleadosSeleccionados = new();
        private string? _primerEmpleadoSeleccionado;
        private Models.Custom.EmpleadosLista? _empleadoSeleccionado;

        private bool _suspendRefresh;
        private bool _initialized;

        public EmpleadosPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.EmpleadosVM>();
            if (ViewModel != null)
                this.BindingContext = ViewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ListadoEmpleados":
                    if (_suspendRefresh) return;

                    // No borres selección ni cambies el texto si ya hay selección
                    emptyStateLabel.IsVisible = ViewModel?.ListadoEmpleados.Count == 0;
                    if (_empleadoSeleccionado == null && _empleadosSeleccionados.Count == 0)
                    {
                        empleadosSeleccionadosStack.Children.Clear();
                        btnSeleccionarEmpleado.Text = "Seleccionar empleado";
                    }
                    break;
            }
        }

        private async void OnSeleccionarEmpleadoClicked(object sender, EventArgs e)
        {
            if (ViewModel?.ListadoEmpleados == null || ViewModel.ListadoEmpleados.Count == 0)
            {
                await DisplayAlert("Aviso", "No hay empleados cargados.", "OK");
                return;
            }

            _suspendRefresh = true;
            var modal = new SelectEmpleadoModal(ViewModel.ListadoEmpleados);
            var seleccionado = await modal.ShowAsync(Navigation);
            if (seleccionado != null)
            {
                _empleadoSeleccionado = seleccionado;
                btnSeleccionarEmpleado.Text = seleccionado.Empleado ?? "Empleado seleccionado";
            }
            _suspendRefresh = false;
        }

        private async void OnAñadirEmpleadoClicked(object sender, EventArgs e)
        {
            var empleadoSeleccionado = _empleadoSeleccionado;
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

            var item = new HorizontalStackLayout { Spacing = 10, Padding = new Thickness(0, 5) };
            item.Children.Add(new Label { Text = "•", TextColor = Color.FromArgb("#2160AB"), FontSize = 16, VerticalOptions = LayoutOptions.Center });
            item.Children.Add(new Label { Text = empleadoSeleccionado.Empleado, TextColor = Colors.White, FontSize = 16, VerticalOptions = LayoutOptions.Center });
            empleadosSeleccionadosStack.Children.Add(item);

            if (_primerEmpleadoSeleccionado == null)
            {
                _primerEmpleadoSeleccionado = empleadoSeleccionado.Empleado ?? string.Empty;
                Helpers.StaticSettings.FijarConfiguracion("Empleado", _primerEmpleadoSeleccionado);
            }
            _empleadoSeleccionado = null;
            btnSeleccionarEmpleado.Text = "Seleccionar empleado";
        }

        private async void OnEmpezarRutaClicked(object sender, EventArgs e)
        {
            if (_empleadosSeleccionados.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar al menos un empleado para continuar.", "OK");
                return;
            }

            if (ViewModel == null || ViewModel.IdRutaActual == Guid.Empty)
            {
                await DisplayAlert("Error", "No se encontró la ruta actual. Asegúrate de que esté cargada.", "OK");
                return;
            }

            var hoy = DateTime.Now.Date;
            var mañana = hoy.AddDays(1);
            var ventasDelDia = (await ViewModel._localDb.ObtenerVentasAsync())
                .Where(v => v.Fecha >= hoy && v.Fecha < mañana)
                .ToList();

            int vueltaActual = ventasDelDia.Count + 1;

            var nuevaVenta = new VentaGeneralLocal
            {
                IdVentaGeneral = Guid.NewGuid(),
                IdRuta = ViewModel.IdRutaActual,
                Fecha = DateTime.Now,
                Vuelta = vueltaActual,
                IdStatusVenta = Guid.NewGuid(),
                Sincronizado = false
            };

            await ViewModel._localDb.GuardarVentaAsync(nuevaVenta);
            await ViewModel._localDb.ImprimirVentasDebugAsync();

            await Navigation.PushAsync(new ClientePage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            empleadosSeleccionadosStack.Opacity = 0;
            await empleadosSeleccionadosStack.FadeTo(1, 600, Easing.CubicIn);

            if (_suspendRefresh) return;

            // Cargar solo la primera vez
            if (!_initialized && ViewModel != null)
            {
                await ViewModel.CargarEmpleadosDesdeLocal();
                _initialized = true;
            }
        }
    }
}