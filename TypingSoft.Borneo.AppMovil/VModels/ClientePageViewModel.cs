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

            // 1. Verificar o crear venta general activa
            var ventaGeneral = await _localDb.ObtenerVentaGeneralActiva();
            if (ventaGeneral == null)
            {
                Guid? idRuta = await _localDb.ObtenerIdRutaAsync();
                ventaGeneral = new Local.VentaGeneralLocal
                {
                    IdVentaGeneral = Guid.NewGuid(),
                    IdRuta = idRuta ?? Guid.Empty,
                    Vuelta = 1,
                    Fecha = DateTime.Now,
                    Sincronizado = false
                };
                await _localDb.GuardarVentaGeneral(ventaGeneral);
            }

            // 2. Obtener IDs relacionados
            var productos = await _localDb.ObtenerProductosAsync();
            var condiciones = await _localDb.ObtenerCondicionesAsync();
            var formas = await _localDb.ObtenerFormasAsync();

            var producto = productos.FirstOrDefault();
            var condicion = condiciones.FirstOrDefault();
            var forma = formas.FirstOrDefault();

            if (producto == null || condicion == null || forma == null)
            {
                await MostrarAlertaAsync("Error", "No hay productos, condiciones o formas de pago disponibles.");
                return;
            }

            // 3. Crear el detalle de venta
            var detalle = new Local.VentaDetalleLocal
            {
                IdDetalle = Guid.NewGuid(),
                IdVentaGeneral = ventaGeneral.IdVentaGeneral,
                IdProducto = producto.Id,
                Cantidad = 1,
                ImporteTotal = 0,
                IdClienteAsociado = cliente.IdClienteAsociado,
                IdCondicionPago = condicion.IdCondicion,
                IdFormaPago = forma.IdForma
            };

            await _localDb.InsertarVentaDetalleAsync(detalle);

            // 4. Añadir cliente a la lista visual si no está
            if (!ClientesASurtir.Any(c => c.IdCliente == cliente.IdCliente))
                ClientesASurtir.Add(cliente);

            // Guarda el IdClienteAsociado del cliente seleccionado
            Helpers.Settings.IdClienteAsociado = cliente.IdClienteAsociado;

            // Mostrar la lista actual de clientes a surtir
            var nombres = string.Join("\n", ClientesASurtir.Select(c => c.Cliente));
            await Application.Current.MainPage.DisplayAlert("Clientes a Surtir", nombres, "OK");
        }

        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            // Puedes implementar tu lógica de alerta aquí si lo deseas
            return Task.CompletedTask;
        }
    }
}
