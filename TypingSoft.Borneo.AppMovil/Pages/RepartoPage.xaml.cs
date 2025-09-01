using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Diagnostics;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Pages.Modals;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class RepartoPage : ContentPage
    {
        VModels.RepartoVM ViewModel;
        private PreciosGeneralesLocal? _productoSeleccionado;

        private bool _suspendRefresh;
        private bool _initialized;

        public RepartoPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.RepartoVM>();
            if (ViewModel != null)
                this.BindingContext = ViewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { }

        private async void OnSeleccionarProductoClicked(object sender, EventArgs e)
        {
            if (ViewModel?.ListadoPreciosLocal == null || ViewModel.ListadoPreciosLocal.Count == 0)
            {
                await DisplayAlert("Aviso", "No hay productos cargados.", "OK");
                return;
            }

            _suspendRefresh = true;
            var modal = new SelectProductoModal(ViewModel.ListadoPreciosLocal);
            var seleccionado = await modal.ShowAsync(Navigation);

            if (seleccionado != null)
            {
                _productoSeleccionado = seleccionado;
                btnSeleccionarProducto.Text = seleccionado.Producto ?? "Producto seleccionado";
            }
            _suspendRefresh = false;
        }

        private async void OnAñadirProductoClicked(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            var productoSeleccionado = _productoSeleccionado;
            var cantidadTexto = cantidadEntry.Text;

            if (productoSeleccionado == null || string.IsNullOrEmpty(cantidadTexto) || !int.TryParse(cantidadTexto, out int cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Aviso", "Seleccione un producto y una cantidad válida.", "OK");
                return;
            }

            if (!decimal.TryParse(productoSeleccionado.Precio, out decimal precioUnitario))
            {
                await DisplayAlert("Error", "El precio del producto no es válido.", "OK");
                return;
            }

            decimal importeTotal = cantidad * precioUnitario;

            var idClienteAsociadoStr = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.IdClienteAsociado);
            if (!Guid.TryParse(idClienteAsociadoStr, out Guid idClienteAsociado))
            {
                await DisplayAlert("Error", "No se pudo obtener el cliente asociado.", "OK");
                return;
            }

            await ViewModel.AgregarDetalleVentaAsync(productoSeleccionado, cantidad, importeTotal, idClienteAsociado);

            productosSeleccionadosStack.Children.Add(new Label
            {
                Text = $"{productoSeleccionado.Producto} - Cantidad: {cantidad} - Importe: {importeTotal:C}",
                FontSize = 14,
                TextColor = Color.FromArgb("#333333")
            });

            // Reset tras añadir
            _productoSeleccionado = null;
            btnSeleccionarProducto.Text = "Seleccionar producto";
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
                Debug.WriteLine($"IdDetalle: {d.IdVentaDetalle}, IdVentaGeneral: {d.IdVentaGeneral}, IdProducto: {d.IdProducto}, Cantidad: {d.Cantidad}, ImporteTotal: {d.ImporteTotal}, IdClienteAsociado: {d.IdClienteAsociado}");
            }

            await Navigation.PushAsync(new UtileriasPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_suspendRefresh) return;

            if (!_initialized && ViewModel != null)
            {
                await ViewModel.CargarProductosDesdeLocal();
                await ViewModel.CargarPreciosDesdeLocal();
                _initialized = true;
            }

            productosSeleccionadosStack.Opacity = 0;
            await productosSeleccionadosStack.FadeTo(1, 600, Easing.CubicIn);
        }

        public void LimpiarCamposYListas(bool limpiarSoloProductos = false)
        {
            _productoSeleccionado = null;
            btnSeleccionarProducto.Text = "Seleccionar producto";
            cantidadEntry.Text = string.Empty;
            productosSeleccionadosStack.Children.Clear();
        }
    }
}