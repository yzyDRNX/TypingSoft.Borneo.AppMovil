using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class RepartoVM : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;

        public RepartoVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;
            ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>();
            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>();
            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>();
            ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>();
            ListadoPreciosPreferenciales = new ObservableCollection<Models.Custom.PreciosPreferencialesLista>();
            ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>();
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;


        [ObservableProperty]
        ObservableCollection<Models.Custom.ProductosLista> listadoProductos;

        [ObservableProperty]
        ObservableCollection<Models.Custom.FormasLista> listadoFormas;

        [ObservableProperty]
        ObservableCollection<Models.Custom.CondicionesLista> listadoCondiciones;
        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosPreferencialesLista> listadoPreciosPreferenciales;

        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosGeneralesLista> listadoPreciosGenerales;

        [ObservableProperty]
        ObservableCollection<PreciosGeneralesLocal> listadoPreciosLocal = new();

        public async Task CargarProductosDesdeLocal()
        {
            var productosLocales = await _localDb.ObtenerProductosAsync();

            if (productosLocales != null && productosLocales.Any())
            {
                ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>(
                    productosLocales.Select(p => new Models.Custom.ProductosLista
                    {
                        Id = p.Id,
                        Producto = p.Producto
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando productos locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay productos locales disponibles.");
            }
        }

        public async Task ObtenerFormasAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Formas";
                Procesando = true;

                var (exitoso, mensaje, listaFormas) = await _catalogos.ObtenerFormas();

                if (exitoso)
                {
                    ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>(listaFormas);

                    // Convierte y guarda en SQLite
                    var formasLocales = listaFormas
                        .Select(f => new Local.FormaLocal { IdForma = f.IdForma, Forma = f.Forma })
                        .ToList();

                    await _localDb.GuardarFormasAsync(formasLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }

        public async Task ObtenerCondicionesAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Condiciones";
                Procesando = true;

                var (exitoso, mensaje, listaCondiciones) = await _catalogos.ObtenerCondiciones();

                if (exitoso)
                {
                    ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>(listaCondiciones);

                    // Convierte y guarda en SQLite
                    var condicionesLocales = listaCondiciones
                        .Select(c => new Local.CondicionLocal { IdCondicion = c.IdCondicion, Condicion = c.Condicion })
                        .ToList();

                    await _localDb.GuardarCondicionesAsync(condicionesLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }

        public async Task ObtenerPreciosGeneralesAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Precios";
                Procesando = true;

                Guid IdClienteAsociado = Helpers.Settings.IdClienteAsociado;
                var (exitoso, mensaje, listaPrecios) = await _catalogos.ObtenerPreciosGenerales();

                if (exitoso)
                {
                    ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>(listaPrecios);

                    // Convierte y guarda en SQLite
                    var preciosLocales = listaPrecios
                        .Select(p => new Local.PreciosGeneralesLocal { IdProducto = p.IdProducto, Producto = p.Producto, Precio = p.Precio.ToString() })
                        .ToList();

                    await _localDb.GuardarPreciosGeneralesAsync(preciosLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener Precios.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }

        public async Task CargarPreciosDesdeLocal()
        {
            var preciosLocales = await _localDb.ObtenerPreciosAsync(); // Este método debe devolver List<PrecioLocal>
            if (preciosLocales != null && preciosLocales.Any())
                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(preciosLocales);
            else
                ListadoPreciosLocal.Clear();
        }

        public async Task CargarPreciosPorClienteAsync(Guid idClienteAsociado)
        {
            // 1. Intenta obtener precios preferenciales para el cliente
            var preciosPreferenciales = await _localDb.ObtenerPreciosPreferencialesPorClienteAsync(idClienteAsociado);

            if (preciosPreferenciales != null && preciosPreferenciales.Any())
            {
                // Si hay precios preferenciales, los mostramos
                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(
                    preciosPreferenciales.Select(p => new PreciosGeneralesLocal
                    {
                        IdProducto = p.IdProducto,
                        Producto = p.Producto,
                        Precio = p.Precio
                    })
                );
            }
            else
            {
                // Si no hay, mostramos los precios generales
                var preciosGenerales = await _localDb.ObtenerPreciosGeneralesAsync();
                ListadoPreciosLocal = new ObservableCollection<PreciosGeneralesLocal>(preciosGenerales);

                if (preciosGenerales == null || !preciosGenerales.Any())
                {
                    await MostrarAlertaAsync("Sin datos", "No hay precios generales en la base local.");
                }
                else
                {
                    await MostrarAlertaAsync("OK", $"Se cargaron {preciosGenerales.Count} precios generales.");
                }
            }
        }

        public async Task AgregarDetalleVentaAsync(
            PreciosGeneralesLocal producto, 
            int cantidad, 
            decimal importeTotal, 
            Guid idClienteAsociado)
        {
            var ventaGeneral = await _localDb.ObtenerVentaGeneralActiva();
            if (ventaGeneral == null) return;

            // Guardar en VentaDetalleLocal
            var detalleVenta = new VentaDetalleLocal
            {
                IdVentaDetalle = Guid.NewGuid(),
                IdVentaGeneral = ventaGeneral.IdVentaGeneral,
                IdProducto = producto.IdProducto,
                Cantidad = cantidad,
                ImporteTotal = importeTotal,
                IdClienteAsociado = idClienteAsociado,
                IdCondicionPago = Guid.NewGuid(), // Valor temporal
                IdFormaPago = Guid.NewGuid()      // Valor temporal
            };
            await _localDb.InsertarVentaDetalleAsync(detalleVenta);

            // Guardar en TicketDetalleLocal
            // Busca el ticket cabecera actual (puedes ajustar la lógica según tu flujo)
            var tickets = await _localDb.ObtenerTicketsAsync();
            var ticketCabecera = tickets
                .Where(t => t.IdCliente == idClienteAsociado)
                .OrderByDescending(t => t.Fecha)
                .FirstOrDefault();

            if (ticketCabecera != null)
            {
                var detalleTicket = new TicketDetalleLocal
                {
                    Id = Guid.NewGuid(),
                    IdTicket = ticketCabecera.Id,
                    IdCliente = ticketCabecera.IdCliente,
                    Cliente = ticketCabecera.Cliente,
                    Empleado = ticketCabecera.Empleado,
                    Fecha = ticketCabecera.Fecha,
                    Descripcion = producto.Producto ?? string.Empty,
                    Cantidad = cantidad,
                    ImporteTotal = importeTotal
                };
                await _localDb.InsertarTicketDetalleAsync(detalleTicket);
            }
            // Si no hay ticket cabecera, puedes crear uno nuevo si tu lógica lo requiere
        }

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }

        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
    }
}
