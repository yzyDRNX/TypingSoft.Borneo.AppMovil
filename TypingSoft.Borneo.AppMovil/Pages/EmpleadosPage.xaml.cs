using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class EmpleadosPage : ContentPage
    {
        VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
        private readonly HashSet<Guid> _empleadosSeleccionados = new HashSet<Guid>();

        public EmpleadosPage()
        {
            InitializeComponent();
            SetupViewModel();
        }

        private void SetupViewModel()
        {
            if (App.ServiceProvider != null)
            {
                var viewModel = App.ServiceProvider.GetService<VModels.CatalogosVM>();
                this.BindingContext = viewModel ?? CreateFallbackViewModel();
            }
            else
            {
                this.BindingContext = CreateFallbackViewModel();
            }
        }

        private VModels.CatalogosVM CreateFallbackViewModel()
        {
            var catalogosService = new Services.CatalogosService();
            var catalogosBL = new BL.CatalogosBL(catalogosService);
            var localDb = new Services.LocalDatabaseService();
            return new VModels.CatalogosVM(catalogosBL, localDb);
        }

        private void OnAñadirEmpleadoClicked(object sender, EventArgs e)
        {
            var empleadoSeleccionado = empleadosPicker.SelectedItem as Models.Custom.EmpleadosLista;

            if (empleadoSeleccionado == null)
            {
                DisplayAlert("Advertencia", "Por favor, seleccione un empleado antes de añadirlo.", "OK");
                return;
            }

            if (_empleadosSeleccionados.Contains(empleadoSeleccionado.Id))
            {
                DisplayAlert("Advertencia", "Este empleado ya está en la lista.", "OK");
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
        }

        private async void OnEmpezarRutaClicked(object sender, EventArgs e)
        {
            if (_empleadosSeleccionados.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar al menos un empleado para continuar.", "OK");
                return;
            }

            await Navigation.PushAsync(new ClientePage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel != null)
            {
                await ViewModel.ObtenerEmpleadosAsync();
            }

            // Resetear selección al aparecer
            empleadosPicker.SelectedItem = null;
        }
    }
}