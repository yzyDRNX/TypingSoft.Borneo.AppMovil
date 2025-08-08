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

        private async void OnAñadirProductoClicked(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            var productoSeleccionado = productosPicker.SelectedItem as TypingSoft.Borneo.AppMovil.Local.PreciosGeneralesLocal;
            var cantidadTexto = cantidadEntry.Text;

            if (productoSeleccionado == null || string.IsNullOrEmpty(cantidadTexto) || !int.TryParse(cantidadTexto, out int cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Aviso", "Por favor seleccione un producto y una cantidad válida.", "OK");
                return;
            }

            if (!decimal.TryParse(productoSeleccionado.Precio, out decimal precioUnitario))
            {
                await DisplayAlert("Error", "El precio del producto no es válido.", "OK");
                return;
            }

            decimal importeTotal = cantidad * precioUnitario;

            // --- CORREGIDO: Obtener el idClienteAsociado como string y convertir a Guid ---
            var idClienteAsociadoStr = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.IdClienteAsociado);
            if (!Guid.TryParse(idClienteAsociadoStr, out Guid idClienteAsociado))
            {
                await DisplayAlert("Error", "No se pudo obtener el cliente asociado.", "OK");
                return;
            }

            await ViewModel.AgregarDetalleVentaAsync(productoSeleccionado, cantidad, importeTotal, idClienteAsociado);

            // Mostrar en la UI
            var productoLabel = new Label
            {
                Text = $"{productoSeleccionado.Producto} - Cantidad: {cantidad} - Importe: {importeTotal:C}",
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

            var detalles = await ViewModel._localDb.ObtenerDetallesAsync();
            foreach (var d in detalles)
            {
                System.Diagnostics.Debug.WriteLine($"IdDetalle: {d.IdVentaDetalle}, IdVentaGeneral: {d.IdVentaGeneral}, IdProducto: {d.IdProducto}, Cantidad: {d.Cantidad}, ImporteTotal: {d.ImporteTotal}, IdClienteAsociado: {d.IdClienteAsociado}");
            }

            await App.NavigationService.Navegar(nameof(UtileriasPage));
            await Navigation.PopAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            productosSeleccionadosStack.Opacity = 0;
            await productosSeleccionadosStack.FadeTo(1, 600, Easing.CubicIn);
        }


        public void LimpiarCamposYListas(bool limpiarSoloProductos = false)
        {
            productosPicker.SelectedItem = null;
            cantidadEntry.Text = string.Empty;
            productosSeleccionadosStack.Children.Clear();
            // Si tienes una lista de productos en el ViewModel, límpiala aquí

            if (!limpiarSoloProductos)
            {
                // Si tuvieras que limpiar el cliente, lo harías aquí
            }
        }
    }
}