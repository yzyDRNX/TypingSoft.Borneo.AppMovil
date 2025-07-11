﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        public UtileriasPageViewModel()
        {
            _localDb = new LocalDatabaseService();
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
          //  _ = CargarUltimoTicketAsync();
            _ = CargarVentaActualYProductos();

            // Recupera el cliente seleccionado correctamente
            string nombreCliente = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.Cliente);
            if (!string.IsNullOrEmpty(nombreCliente))
            {
                VentaActual = new TicketDetalleLocal { Cliente = nombreCliente };
            }
        }

        private async void CargarDescripcionRuta()
        {
            DescripcionRuta = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        //private async Task CargarUltimoTicketAsync()
        //{
        //    var tickets = await _localDb.ObtenerTicketsAsync();
        //    _ultimoTicket = tickets?.OrderByDescending(t => t.IdCliente).FirstOrDefault();
        //}

        public async Task CargarVentaActualYProductos()
        {
            var tickets = await _localDb.ObtenerTicketsAsync();
            var ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();
            if (ultimoTicket != null)
            {
                VentaActual = ultimoTicket;
                OnPropertyChanged(nameof(NombreCliente));

                var detalles = await _localDb.ObtenerDetallesPorTicketAsync(ultimoTicket.Id);
                // Agrupar por descripción y precio unitario
                var agrupados = detalles
                .GroupBy(d => new {
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

        private async Task ImprimirTicketAsync(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles)
        {
            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
            if (printer != null)
            {
                if (_numeroImpresiones > 2)
                {
                    await App.Current.MainPage.DisplayAlert("Advertencia", "No se puede reimprimir", "OK");
                }
                else
                {
                    // Obtener el IdClienteAsociado (puede ser IdCliente en tu modelo)
                    var idClienteAsociado = ticket.IdCliente;
                    var aplicaMuestraPrecio = await _localDb.ObtenerAplicaMuestraPrecioPorClienteAsociadoAsync(idClienteAsociado);
                    bool mostrarPrecio = aplicaMuestraPrecio ?? true; // Si es null, muestra el precio por defecto

                    string ticketOriginal = TicketFormatter.FormatearTicketLocal(ticket, detalles, _numeroImpresiones, mostrarPrecio);
                    await printer.PrintTextAsync(ticketOriginal);
                    _numeroImpresiones++;
                }
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
            if (VentaActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta actual cargada.", "OK");
                return;
            }

            var detalles = await _localDb.ObtenerDetallesPorTicketAsync(VentaActual.Id);

            if (detalles == null || detalles.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay productos para imprimir.", "OK");
                return;
            }

            await ImprimirTicketAsync(VentaActual, detalles);
        }


        [RelayCommand]
        public async Task OtraVentaMismoClienteAsync()
        {
            if (VentaActual == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay venta activa.", "OK");
                return;
            }
            _numeroImpresiones = 1; // Reinicia el contador de impresiones
            // Simplemente volvemos a la captura de productos
            await App.Current.MainPage.Navigation.PushAsync(new Pages.RepartoPage());

            // Una vez en RepartoPage solo limpiamos los campos de producto
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.RepartoPage page)
                page.LimpiarCamposYListas(true);
        }

        [RelayCommand]
        public async Task SiguienteEntregaAsync()
        {
            VentaActual = null; // Limpia la venta actual
            _numeroImpresiones = 1; // Reinicia el contador de impresiones

            Productos.Clear(); // Limpia los productos
            OnPropertyChanged(nameof(Productos));
            OnPropertyChanged(nameof(Total));

            // Navega a la pantalla de selección de cliente
            await App.Current.MainPage.Navigation.PushAsync(new Pages.EmpleadosPage());

            // Espera a que la navegación termine y luego limpia la UI de ClientePage
            if (App.Current.MainPage.Navigation.NavigationStack.LastOrDefault() is Pages.ClientePage page)
                page.LimpiarCamposYListas();
        }

    }

    public class ProductoVentaDTO
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
