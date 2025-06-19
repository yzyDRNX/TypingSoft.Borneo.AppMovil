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
        private int _numeroImpresiones = 0;
        private TicketLocal _ultimoTicket;
        private TicketLocal _ventaActual;
        public TicketLocal VentaActual
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

        public UtileriasPageViewModel()
        {
            _localDb = new LocalDatabaseService();
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
            _ = CargarUltimoTicketAsync();
            _ = CargarVentaActualYProductos();

            // Recupera el cliente seleccionado correctamente
            string nombreCliente = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.Cliente);
            if (!string.IsNullOrEmpty(nombreCliente))
            {
                VentaActual = new TicketLocal { Cliente = nombreCliente };
            }
        }

        private async void CargarDescripcionRuta()
        {
            DescripcionRuta = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        private async Task CargarUltimoTicketAsync()
        {
            var tickets = await _localDb.ObtenerTicketsAsync();
            _ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();
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
                Productos.Clear();
                foreach (var d in detalles)
                {
                    Productos.Add(new ProductoVentaDTO
                    {
                        Nombre = d.Descripcion,
                        Cantidad = d.Cantidad,
                        Precio = d.Importe / (d.Cantidad == 0 ? 1 : d.Cantidad) // Evita división por cero
                    });
                }
                OnPropertyChanged(nameof(Productos));
                OnPropertyChanged(nameof(Total));
            }
        }

        private async Task ImprimirTicketAsync(TicketLocal ticket)
        {
            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
            if (printer != null)
            {
                // 1. Obtén los detalles del ticket
                var detalles = await _localDb.ObtenerDetallesPorTicketAsync(ticket.Id);

                // 2. Imprime ORIGINAL
                string ticketOriginal = TicketFormatter.FormatearTicketLocal(ticket, detalles, 1);
                await printer.PrintTextAsync(ticketOriginal);

                // 3. Imprime REIMPRESION
                string ticketReimpresion = TicketFormatter.FormatearTicketLocal(ticket, detalles, 2);
                await printer.PrintTextAsync(ticketReimpresion);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se encontró el servicio de impresión.", "OK");
            }
        }

        // Comando de impresión
        [RelayCommand]
        public async Task ImprimirAsync()
        {
            if (_ultimoTicket == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay ticket para imprimir.", "OK");
                return;
            }

            await ImprimirTicketAsync(_ultimoTicket);
        }

        [RelayCommand]
        public async Task OtraVentaMismoClienteAsync()
        {
            var clienteActual = VentaActual?.Cliente;
            if (string.IsNullOrEmpty(clienteActual))
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay cliente seleccionado.", "OK");
                return;
            }

            var nuevoTicket = new TicketLocal
            {
                Id = Guid.NewGuid(),
                Cliente = clienteActual,
                Fecha = DateTime.Now
            };
            await _localDb.InsertarTicketAsync(nuevoTicket);

            // Navega a la pantalla de reparto para capturar productos
            await App.Current.MainPage.Navigation.PushAsync(new Pages.RepartoPage());

            // Espera a que la navegación termine y luego limpia solo productos
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.RepartoPage page)
                page.LimpiarCamposYListas(true); // true = solo productos
        }

        [RelayCommand]
        public async Task SiguienteEntregaAsync()
        {
            VentaActual = null;

            // Navega a la pantalla de selección de cliente
            await App.Current.MainPage.Navigation.PushAsync(new Pages.ClientePage());

            // Espera a que la navegación termine y luego limpia
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.ClientePage page)
                page.LimpiarCamposYListas(); // O page.LimpiarTodo() si así se llama tu método
        }
    }

    public class ProductoVentaDTO
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
