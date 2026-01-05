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
        private TicketDetalleLocal _ventaActual;
        public TicketDetalleLocal VentaActual
        {
            get => _ventaActual;
            set { _ventaActual = value; OnPropertyChanged(); }
        }

        private string _fechaActual;
        public string FechaActual
        {
            get => _fechaActual;
            set { _fechaActual = value; OnPropertyChanged(); }
        }

        private string _descripcionRuta;
        public string DescripcionRuta
        {
            get => _descripcionRuta;
            set { _descripcionRuta = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ProductoVentaDTO> Productos { get; } = new();
        public string NombreCliente => VentaActual?.Cliente ?? string.Empty;
        public decimal Total => Productos.Sum(p => p.Precio * p.Cantidad);

        public ObservableCollection<string> ImpresorasBluetooth { get; } = new();

        [ObservableProperty]
        private string impresoraSeleccionada;

        // Folio reservado para esta impresión (se asigna solo una vez)
        private int? _folioImpresion;

        public UtileriasPageViewModel()
        {
            _localDb = new LocalDatabaseService();
            var ventaSession = App.ServiceProvider.GetService<VentaSessionServices>();
            VentaActual = ventaSession?.TicketActual;
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
            _ = CargarVentaActualYProductos();

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
            if (ultimoTicket == null) return;

            VentaActual = ultimoTicket;
            OnPropertyChanged(nameof(NombreCliente));

            var detalles = await _localDb.ObtenerDetallesPorTicketAsync(ultimoTicket.Id);
            //var agrupados = detalles
            //    .GroupBy(d => new
            //    {
            //        Descripcion = d.Descripcion,
            //        PrecioUnitario = d.Cantidad == 0 ? 0m : d.ImporteTotal / d.Cantidad
            //    })
            //    .Select(g => new ProductoVentaDTO
            //    {
            //        Nombre = g.Key.Descripcion,
            //        Cantidad = g.Sum(x => x.Cantidad),
            //        Precio = g.Key.PrecioUnitario
            //    });

            ProductoVentaDTO grupo=new ProductoVentaDTO();
            grupo.Nombre = detalles.Descripcion;
            grupo.Cantidad = detalles.Cantidad;
            grupo.Precio = detalles.ImporteTotal / detalles.Cantidad;


            Productos.Clear();
            //foreach (var p in detalles)
                Productos.Add(grupo);

            OnPropertyChanged(nameof(Productos));
            OnPropertyChanged(nameof(Total));
        }

        [RelayCommand]
        public async Task ImprimirOriginalAsync()
        {
            if (string.IsNullOrEmpty(ImpresoraSeleccionada))
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Selecciona una impresora Bluetooth.", "OK");
                return;
            }
            if (VentaActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta/ticket actual.", "OK");
                return;
            }
            var detalles = await _localDb.ObtenerDetallesPorTicketAsync(VentaActual.Id);
            if (detalles == null )
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay productos para imprimir.", "OK"); 
                return;
            }

            await ImprimirTicketOriginalAsync(VentaActual, detalles, ImpresoraSeleccionada);
        }

        [RelayCommand]
        public async Task ImprimirCopiaAsync()
        {
            if (string.IsNullOrEmpty(ImpresoraSeleccionada))
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Selecciona una impresora Bluetooth.", "OK");
                return;
            }
            if (VentaActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta/ticket actual.", "OK");
                return;
            }
            var detalles = await _localDb.ObtenerDetallesPorTicketAsync(VentaActual.Id);
            if (detalles == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay productos para imprimir.", "OK");
                return;
            }

            await ImprimirTicketCopiaAsync(VentaActual, detalles, ImpresoraSeleccionada);
        }

        private async Task ImprimirTicketOriginalAsync(TicketDetalleLocal ticket, TicketDetalleLocal detalles, string printerName)
        {
            var printer = new RawBtPrinter(printerName);
       

            var aplicaMuestraPrecio = await _localDb.ObtenerAplicaMuestraPrecioPorClienteAsociadoAsync(ticket.IdCliente);
            bool mostrarPrecio = aplicaMuestraPrecio ?? true;

            // AplicaAPP ya no controla la visibilidad en ticket: siempre mostramos producto
            bool mostrarProducto = true;

           
                _folioImpresion = await ReservarSiguienteFolioAsync();
          
            string ticketTexto = await TicketFormatter.FormatearTicketLocalAsync(
                _localDb, ticket, detalles, 1, mostrarPrecio, mostrarProducto, _folioImpresion.Value);

            ticketTexto += "\n\n\n\n\n";

            try
            {
                await printer.PrintTextAsync(ticketTexto);

                // Comando de corte (puede fallar si la impresora no lo soporta: lo tratamos igual, sin cerrar la app)
                byte[] cutCommand = { 0x1D, 0x56, 0x00 };
                await printer.PrintBytesAsync(cutCommand);

            }
            catch (Exception ex)
            {
                var mensaje = $"No se pudo imprimir en '{printerName}'. " +
                              "Verifica que la impresora esté encendida, dentro del alcance y emparejada.\n\n" +
                              $"Detalle: {ex.Message}";
                await App.Current.MainPage.DisplayAlert("Impresión", mensaje, "OK");
            }
        }
        private async Task ImprimirTicketCopiaAsync(TicketDetalleLocal ticket, TicketDetalleLocal detalles, string printerName)
        {
            var printer = new RawBtPrinter(printerName);
          

            var aplicaMuestraPrecio = await _localDb.ObtenerAplicaMuestraPrecioPorClienteAsociadoAsync(ticket.IdCliente);
            bool mostrarPrecio = aplicaMuestraPrecio ?? true;

            // AplicaAPP ya no controla la visibilidad en ticket: siempre mostramos producto
            bool mostrarProducto = true;


            string ticketTexto = await TicketFormatter.FormatearTicketLocalAsync(
                _localDb, ticket, detalles, 2, mostrarPrecio, mostrarProducto, _folioImpresion.Value);

            ticketTexto += "\n\n\n\n\n";

            try
            {
                await printer.PrintTextAsync(ticketTexto);

                // Comando de corte (puede fallar si la impresora no lo soporta: lo tratamos igual, sin cerrar la app)
                byte[] cutCommand = { 0x1D, 0x56, 0x00 };
                await printer.PrintBytesAsync(cutCommand);

                _numeroImpresiones++;
            }
            catch (Exception ex)
            {
                var mensaje = $"No se pudo imprimir en '{printerName}'. " +
                              "Verifica que la impresora esté encendida, dentro del alcance y emparejada.\n\n" +
                              $"Detalle: {ex.Message}";
                await App.Current.MainPage.DisplayAlert("Impresión", mensaje, "OK");
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

            _folioImpresion = null;
            _numeroImpresiones = 1;

            await App.Current.MainPage.Navigation.PushAsync(new Pages.RepartoPage());
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.RepartoPage page)
                page.LimpiarCamposYListas(true);
        }

        [RelayCommand]
        public async Task SiguienteEntregaAsync()
        {
            // Garantiza folio por venta concluida si no se imprimió
            var ventaActiva = await _localDb.ObtenerVentaGeneralActiva();
            if (ventaActiva != null)
            {
                var dets = await _localDb.ObtenerDetallesPorVentaGeneralAsync(ventaActiva.IdVentaGeneral);
                if (dets?.Count > 0 && dets.All(d => d.ValorFolioVenta <= 0))
                {
                    var folio = await _localDb.ReservarIncrementarFolioAsync();
                    await _localDb.AsignarFolioALaVentaActivaAsync(folio);
                }
            }

            _folioImpresion = null;
            _numeroImpresiones = 1;

            var clienteVm = App.ServiceProvider.GetService<VModels.ClientePageViewModel>();
            clienteVm?.ClientesASurtir.Clear();

            var ventaSession = App.ServiceProvider.GetService<VentaSessionServices>();
            if (ventaSession != null)
            {
                ventaSession.TicketActual = null;
                ventaSession.VentaGeneralActual = null;
            }

            VentaActual = null;
            Productos.Clear();
            OnPropertyChanged(nameof(NombreCliente));
            OnPropertyChanged(nameof(Productos));
            OnPropertyChanged(nameof(Total));

            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdCliente, string.Empty);
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdClienteAsociado, string.Empty);
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.Cliente, string.Empty);

            await App.Current.MainPage.Navigation.PushAsync(new Pages.ClientePage());
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.ClientePage clientePageInstance)
                clientePageInstance.LimpiarCamposYListas();
        }

        // Método único que obtiene el folio de la venta (si existe) o lo reserva (si faltó)
        private async Task<int> ReservarSiguienteFolioAsync()
        {
            // Si la venta activa ya tiene folio en sus detalles, úsalo
            var venta = await _localDb.ObtenerVentaGeneralActiva();
            if (venta != null)
            {
                var detallesVenta = await _localDb.ObtenerDetallesPorVentaGeneralAsync(venta.IdVentaGeneral);
                var folioAsignado = detallesVenta?.Where(x => x.ValorFolioVenta > 0).Select(x => x.ValorFolioVenta).DefaultIfEmpty(0).Max() ?? 0;
                if (folioAsignado > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[FOLIO] Utilerías: usando folio ya asignado {folioAsignado}");
                    return folioAsignado;
                }
            }

            // Si no tiene folio asignado (edge case), resérvalo ahora y persiste
            var nuevoFolio = await _localDb.ReservarIncrementarFolioAsync();
            await _localDb.AsignarFolioALaVentaActivaAsync(nuevoFolio);
            System.Diagnostics.Debug.WriteLine($"[FOLIO] Utilerías: reservado y asignado folio {nuevoFolio}");
            return nuevoFolio;
        }

        [RelayCommand]
        public async Task VolverMenu()
        {
            await Navegacion.Navegar(nameof(Pages.MenuPage));
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
