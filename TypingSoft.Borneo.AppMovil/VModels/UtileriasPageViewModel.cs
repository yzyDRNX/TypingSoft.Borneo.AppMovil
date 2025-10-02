using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;
using System.Linq;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class UtileriasPageViewModel : Helpers.VMBase
    {
        private readonly LocalDatabaseService _localDb;
        private int _numeroImpresiones = 1;
        private TicketDetalleLocal _ultimoTicket;
        private TicketDetalleLocal _ventaActual;
        public TicketDetalleLocal VentaActual
        {
            get => _ventaActual;
            set
            {
                _ventaActual = value;
                OnPropertyChanged();
            }
        }
        private string _fechaActual;
        public string FechaActual
        {
            get => _fechaActual;
            set
            {
                _fechaActual = value;
                OnPropertyChanged();
            }
        }

        private string _descripcionRuta;
        public string DescripcionRuta
        {
            get => _descripcionRuta;
            set
            {
                _descripcionRuta = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductoVentaDTO> Productos { get; set; } = new();
        public string NombreCliente => VentaActual?.Cliente ?? string.Empty;
        public decimal Total => Productos.Sum(p => p.Precio * p.Cantidad);

        public ObservableCollection<string> ImpresorasBluetooth { get; set; } = new();

        [ObservableProperty]
        private string impresoraSeleccionada;

        public UtileriasPageViewModel()
        {
            _localDb = new LocalDatabaseService();
            var ventaSession = App.ServiceProvider.GetService<VentaSessionServices>();
            VentaActual = ventaSession?.TicketActual; // mantiene IdCliente (IdClienteAsociado)
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
            _ = CargarVentaActualYProductos();

            // Solo actualiza el nombre visible, no remplaza el objeto (evita perder IdCliente)
            string nombreCliente = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.Cliente);
            if (!string.IsNullOrEmpty(nombreCliente) && VentaActual != null)
            {
                VentaActual.Cliente = nombreCliente;
                OnPropertyChanged(nameof(NombreCliente));
            }
        }

        public void CargarImpresorasBluetooth()
        {
            var lista = RawBtPrinter.GetBondedPrinterNames();
            ImpresorasBluetooth.Clear();
            foreach (var nombre in lista)
                ImpresorasBluetooth.Add(nombre);

            var impresoraGuardada = Helpers.Settings.ObtenerValor<string>("ImpresoraBluetooth");
            if (!string.IsNullOrEmpty(impresoraGuardada) && lista.Contains(impresoraGuardada))
                ImpresoraSeleccionada = impresoraGuardada;
            else if (lista.Count > 0)
                ImpresoraSeleccionada = lista[0];
        }

        private async void CargarDescripcionRuta()
        {
            DescripcionRuta = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        public async Task CargarVentaActualYProductos()
        {
            var tickets = await _localDb.ObtenerTicketsAsync();
            var ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();
            if (ultimoTicket != null)
            {
                VentaActual = ultimoTicket;
                OnPropertyChanged(nameof(NombreCliente));

                var detalles = await _localDb.ObtenerDetallesPorTicketAsync(ultimoTicket.Id);
                var agrupados = detalles
                    .GroupBy(d => new
                    {
                        Descripcion = d.Descripcion,
                        PrecioUnitario = d.Cantidad == 0 ? 0m : d.ImporteTotal / d.Cantidad
                    })
                    .Select(g => new ProductoVentaDTO
                    {
                        Nombre = g.Key.Descripcion,
                        Cantidad = g.Sum(x => x.Cantidad),
                        Precio = g.Key.PrecioUnitario
                    });

                Productos.Clear();
                foreach (var p in agrupados)
                    Productos.Add(p);

                OnPropertyChanged(nameof(Productos));
                OnPropertyChanged(nameof(Total));
            }
        }

        // Comando de impresión
        [RelayCommand]
        public async Task ImprimirAsync()
        {
            if (string.IsNullOrEmpty(ImpresoraSeleccionada))
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Selecciona una impresora Bluetooth.", "OK");
                return;
            }

            var ticketActual = VentaActual;
            if (ticketActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta/ticket actual.", "OK");
                return;
            }

            var detalles = await _localDb.ObtenerDetallesPorTicketAsync(ticketActual.Id);
            if (detalles == null || detalles.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay productos para imprimir.", "OK");
                return;
            }

            await ImprimirTicketAsync(ticketActual, detalles, ImpresoraSeleccionada);
        }

        private async Task ImprimirTicketAsync(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, string printerName)
        {
            var printer = new RawBtPrinter(printerName);
            if (_numeroImpresiones > 2)
            {
                await App.Current.MainPage.DisplayAlert("Advertencia", "No se puede reimprimir", "OK");
            }
            else
            {
                var idClienteAsociado = ticket.IdCliente; // contiene el IdClienteAsociado
                var aplicaMuestraPrecio = await _localDb.ObtenerAplicaMuestraPrecioPorClienteAsociadoAsync(idClienteAsociado);
                bool mostrarPrecio = aplicaMuestraPrecio ?? true;

                string ticketTexto = await TicketFormatter.FormatearTicketLocalAsync(_localDb, ticket, detalles, _numeroImpresiones, mostrarPrecio);
                ticketTexto += "\n\n\n\n\n";

                await printer.PrintTextAsync(ticketTexto);

                byte[] cutCommand = new byte[] { 0x1D, 0x56, 0x00 };
                await printer.PrintBytesAsync(cutCommand);      

                _numeroImpresiones++;
            }
        }

        [RelayCommand]
        public async Task OtraVentaMismoClienteAsync()
        {
            if (VentaActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta activa.", "OK");
                return;
            }

            await InsertarValoresAppVentaDetalleAsync();

            _numeroImpresiones = 1;
            await App.Current.MainPage.Navigation.PushAsync(new Pages.RepartoPage());
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.RepartoPage page)
                page.LimpiarCamposYListas(true);
        }

        [RelayCommand]
        public async Task SiguienteEntregaAsync()
        {
            // Genera el siguiente folio (manteniendo la lógica existente)
            await InsertarValoresAppVentaDetalleAsync();

            // 1) Limpiar estado de cliente (pero conservar el empleado)
            var clienteVm = App.ServiceProvider.GetService<TypingSoft.Borneo.AppMovil.VModels.ClientePageViewModel>();
            clienteVm?.ClientesASurtir.Clear();

            // 2) Reset de venta actual y productos
            var ventaSession = App.ServiceProvider.GetService<VentaSessionServices>();
            if (ventaSession != null)
            {
                ventaSession.TicketActual = null;
                ventaSession.VentaGeneralActual = null;
            }

            VentaActual = null;
            Productos.Clear();
            _numeroImpresiones = 1;
            OnPropertyChanged(nameof(NombreCliente));
            OnPropertyChanged(nameof(Productos));
            OnPropertyChanged(nameof(Total));

            // 3) Limpiar settings SOLO del cliente (empleado se conserva)
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdCliente, string.Empty);
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdClienteAsociado, string.Empty);
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.Cliente, string.Empty);

            // IMPORTANTE: No volver a cargar último ticket (antes lo hacía y revertía el estado)
            // await CargarVentaActualYProductos();  // <-- Eliminado a propósito

            // 4) Navegar directamente a la página de selección de cliente
            await App.Current.MainPage.Navigation.PushAsync(new Pages.ClientePage());

            // 5) Limpiar visual del nuevo ClientePage (por si arrastra algo del binding inicial)
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.ClientePage clientePageInstance)
                clientePageInstance.LimpiarCamposYListas();
        }

        private async Task InsertarValoresAppVentaDetalleAsync()
        {
            var idRuta = await _localDb.ObtenerIdRutaAsync() ?? Guid.Empty;
            int ultimoFolio = await _localDb.ObtenerUltimoValorFolioVentaAsync();
            var detalle = new ValoresAppVentaDetalleLocal
            {
                Id = Guid.NewGuid(),
                IdRuta = idRuta,
                ValorFolioVenta = ultimoFolio + 1,
                SerieVentaDetalle = "S",
                UltimaActualizacion = DateTime.Now
            };
            await _localDb.InsertarValoresAppVentaDetalleAsync(detalle);
        }

        partial void OnImpresoraSeleccionadaChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                Helpers.Settings.FijarConfiguracion("ImpresoraBluetooth", value);
        }
    }

    public class ProductoVentaDTO
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
