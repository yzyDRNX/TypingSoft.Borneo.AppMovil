using Microsoft.Maui.Controls;
using System;
using TypingSoft.Borneo.AppMovil.Models.API; // Asegúrate de que esta referencia sea correcta
using TypingSoft.Borneo.AppMovil.VModels;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class RepartoPage : ContentPage
    {
        VModels.RepartoVM ViewModel;


        public RepartoPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.RepartoVM>();
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

            }
        }

        private CatalogosVM CreateFallbackViewModel()
        {
            var catalogosService = new Services.CatalogosService();
            var catalogosBL = new BL.CatalogosBL(catalogosService);
            var localDb = new Services.LocalDatabaseService();
            return new CatalogosVM(catalogosBL, localDb);
        }

        private void OnAñadirProductoClicked(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            // Cambio seguro: Usar 'dynamic' para evitar errores de tipo (o reemplaza con tu clase real)
            var productoSeleccionado = productosPicker.SelectedItem as dynamic; // O usa el tipo correcto (ej: Models.Producto)
            var cantidad = cantidadEntry.Text;

            if (productoSeleccionado == null || string.IsNullOrEmpty(cantidad))
            {
                DisplayAlert("Aviso", "Por favor seleccione un producto y una cantidad.", "OK");
                return;
            }

            // Mantener la lógica original de añadir al StackLayout
            var productoLabel = new Label
            {
                Text = $"{productoSeleccionado.Producto} - Cantidad: {cantidad}", // Asegúrate de que 'Producto' sea la propiedad correcta
                FontSize = 14,
                TextColor = Color.FromArgb("#333333")
            };

            productosSeleccionadosStack.Children.Add(productoLabel);
            productosPicker.SelectedItem = null;
            cantidadEntry.Text = string.Empty;
        }

        private async void OnConcluirClicked(object sender, EventArgs e)
        {
            if (productosSeleccionadosStack.Children.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe añadir al menos un producto antes de continuar.", "OK");
                return;
            }

            await DisplayAlert("Éxito", "Reparto concluido correctamente.", "OK");
            await App.NavigationService.Navegar(nameof(UtileriasPage));
            await Navigation.PopAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
            {
                await ViewModel.CargarProductosDesdeLocal();
            }
        }
    }
}