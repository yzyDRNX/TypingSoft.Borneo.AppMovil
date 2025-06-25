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

            // --- GUARDAR DETALLE DEL PRODUCTO EN TicketDetalleLocal ---
            var tickets = await ViewModel._localDb.ObtenerTicketsAsync();
            var ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();

            if (ultimoTicket != null)
            {
                // Inserta un nuevo detalle para este producto
                var detalle = new TypingSoft.Borneo.AppMovil.Local.TicketDetalleLocal
                {
                    TicketId = ultimoTicket.Id,
                    Descripcion = productoSeleccionado.Producto ?? string.Empty,
                    Cantidad = cantidad,
                    Importe = importeTotal
                };
                await ViewModel._localDb.InsertarTicketDetalleAsync(detalle);
            }
            else
            {
                // Si no existe un ticket, crea uno nuevo y luego el detalle
                var nuevoTicket = new TypingSoft.Borneo.AppMovil.Local.TicketLocal
                {
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.Now
                    // Los demás campos pueden quedar vacíos o por defecto
                };
                await ViewModel._localDb.InsertarTicketAsync(nuevoTicket);

                var detalle = new TypingSoft.Borneo.AppMovil.Local.TicketDetalleLocal
                {
                    TicketId = nuevoTicket.Id,
                    Descripcion = productoSeleccionado.Producto ?? string.Empty,
                    Cantidad = cantidad,
                    Importe = importeTotal
                };
                await ViewModel._localDb.InsertarTicketDetalleAsync(detalle);
            }
        }

        private async void OnConcluirClicked(object sender, EventArgs e)
        {
            if (productosSeleccionadosStack.Children.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe añadir al menos un producto antes de continuar.", "OK");
                return;
            }

            // Consultar y mostrar los tickets guardados en la base de datos local
            //var tickets = await ViewModel._localDb.ObtenerTicketsAsync();
            //if (tickets == null || tickets.Count == 0)
            //{
            //    await DisplayAlert("Tickets", "No hay tickets registrados.", "OK");
            //}
            //else
            //{
            //    string resumen = string.Join(Environment.NewLine + Environment.NewLine, tickets.Select(t =>
            //        $"Fecha: {t.Fecha:dd/MM/yyyy HH:mm}\nCliente: {t.Cliente}\nProducto: {t.Descripcion}\nCantidad: {t.Cantidad}\nImporte: {t.ImporteTotal:C}\nEmpleado: {t.Empleado}"
            //    ));

            //    await DisplayAlert("Tickets en BD", resumen, "OK");
            //}

            await App.NavigationService.Navegar(nameof(UtileriasPage));
            await Navigation.PopAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
            {
                var idClienteAsociado = Helpers.StaticSettings.ObtenerValor(Helpers.StaticSettings.IdClienteAsociado);
                await ViewModel.CargarPreciosPorClienteAsync(idClienteAsociado);
            }
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