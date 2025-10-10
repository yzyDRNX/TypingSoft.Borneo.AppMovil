using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;
using System.Linq;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class ClientePageViewModel : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;

        public ClientePageViewModel(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>();
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }

        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;
        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> clientesASurtir = new();

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async Task CargarClientesDesdeLocal()
        {
            Guid? idRutaLocal = await _localDb.ObtenerIdRutaAsync();
            if (!idRutaLocal.HasValue || idRutaLocal.Value == Guid.Empty)
            {
                await MostrarAlertaAsync("Advertencia", "No hay ruta local válida.");
                return;
            }

            var clientesLocales = await _localDb.ObtenerClientesAsync(idRutaLocal.Value);
            if (clientesLocales != null && clientesLocales.Any())
            {
                ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>(
                    clientesLocales.Select(c => new Models.Custom.ClientesLista
                    {
                        IdCliente = c.IdCliente,
                        IdClienteAsociado = c.IdClienteAsociado,
                        Cliente = c.Cliente
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando clientes locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay datos locales.");
            }
        }

        [RelayCommand]
        public async Task Surtir(Models.Custom.ClientesLista cliente)
        {
            if (cliente == null)
                return;

            // 1) Crear nueva venta general
            Guid? idRuta = await _localDb.ObtenerIdRutaAsync();
            var hoy = DateTime.Now.Date;
            var mañana = hoy.AddDays(1);
            var ventasDelDia = (await _localDb.ObtenerVentasAsync())
                .Where(v => v.Fecha >= hoy && v.Fecha < mañana)
                .ToList();
            int vueltaActual = ventasDelDia.Count + 1;

            var nuevaVenta = new VentaGeneralLocal
            {
                IdVentaGeneral = Guid.NewGuid(),
                IdRuta = idRuta ?? Guid.Empty,
                Vuelta = vueltaActual,
                Fecha = DateTime.Now,
                IdStatusVenta = Guid.NewGuid(),
                Sincronizado = false
            };
            await _localDb.GuardarVentaAsync(nuevaVenta);

            // 2) Resolver condición por IdClienteAsociado
            var condicionTexto = await _localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(cliente.IdClienteAsociado);

            // 3) Crear ticket cabecera con snapshot de condición
            var empleadoSeleccionado = Helpers.StaticSettings.ObtenerValor<string>("Empleado");
            var nuevoTicket = new TicketDetalleLocal
            {
                Id = Guid.NewGuid(),
                IdTicket = nuevaVenta.IdVentaGeneral,
                IdCliente = cliente.IdClienteAsociado,
                Cliente = cliente.Cliente ?? string.Empty,
                Empleado = empleadoSeleccionado,
                Fecha = DateTime.Now,
                CondicionPago = condicionTexto
            };
            await _localDb.InsertarTicketAsync(nuevoTicket);

            // 4) Actualizar UI
            if (!ClientesASurtir.Any(c => c.IdCliente == cliente.IdCliente))
                ClientesASurtir.Add(cliente);

            // Persistir el cliente seleccionado para Reparto
            Helpers.Settings.IdClienteAsociado = cliente.IdClienteAsociado;
        }

        public async Task IniciarNuevaVenta(Models.Custom.ClientesLista cliente)
        {
            var ventaSession = App.ServiceProvider.GetService<VentaSessionServices>();

            Guid? idRuta = await _localDb.ObtenerIdRutaAsync();
            var nuevaVenta = new VentaGeneralLocal
            {
                IdVentaGeneral = Guid.NewGuid(),
                IdRuta = idRuta ?? Guid.Empty,
                Vuelta = 1,
                Fecha = DateTime.Now,
                IdStatusVenta = Guid.NewGuid(),
                Sincronizado = false
            };
            await _localDb.GuardarVentaAsync(nuevaVenta);

            var condicionTexto = await _localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(cliente.IdClienteAsociado);

            var empleadoSeleccionado = Helpers.StaticSettings.ObtenerValor<string>("Empleado");
            var nuevoTicket = new TicketDetalleLocal
            {
                Id = Guid.NewGuid(),
                IdTicket = nuevaVenta.IdVentaGeneral,
                IdCliente = cliente.IdClienteAsociado,
                Cliente = cliente.Cliente ?? string.Empty,
                Empleado = empleadoSeleccionado,
                Fecha = DateTime.Now,
                CondicionPago = condicionTexto
            };
            await _localDb.InsertarTicketAsync(nuevoTicket);

            // Guarda el ticket y venta actual en el servicio
            ventaSession.TicketActual = nuevoTicket;
            ventaSession.VentaGeneralActual = nuevaVenta;

            // Persistir también aquí el cliente asociado
            Helpers.Settings.IdClienteAsociado = cliente.IdClienteAsociado;
        }

        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            // Puedes implementar tu lógica de alerta aquí si lo deseas
            return Task.CompletedTask;
        }
    }
}
