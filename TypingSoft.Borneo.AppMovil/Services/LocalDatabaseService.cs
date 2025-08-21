using SQLite;
using System.IO;
using TypingSoft.Borneo.AppMovil.Local;
using ZXing.Datamatrix;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class LocalDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public LocalDatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app_local.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<EmpleadoLocal>().Wait();
            _database.CreateTableAsync<ClienteLocal>().Wait();
            _database.CreateTableAsync<ProductoLocal>().Wait();
            _database.CreateTableAsync<FormaLocal>().Wait();
            _database.CreateTableAsync<CondicionLocal>().Wait();
            _database.CreateTableAsync<PreciosGeneralesLocal>().Wait();
            _database.CreateTableAsync<RutaLocal>().Wait();
            _database.CreateTableAsync<VentaGeneralLocal>().Wait();
            _database.CreateTableAsync<VentaDetalleLocal>().Wait();
            _database.CreateTableAsync<PreciosPreferencialesLocal>().Wait();
            _database.CreateTableAsync<TicketDetalleLocal>().Wait();
            _database.CreateTableAsync<FacturacionLocal>().Wait();
            _database.CreateTableAsync<ClientesAplicacionesLocal>().Wait();
            _database.CreateTableAsync<ValoresAppVentaDetalleLocal>().Wait();
        }
        public async Task InsertarValoresAppVentaDetalleAsync(ValoresAppVentaDetalleLocal detalle)
        {
            await _database.InsertAsync(detalle);
        }
        public async Task<bool?> ObtenerAplicaMuestraPrecioPorClienteAsociadoAsync(Guid idClienteAsociado)
        {
            var clienteApp = await _database.Table<ClientesAplicacionesLocal>()
                .FirstOrDefaultAsync(c => c.IdClienteAsociado == idClienteAsociado);
            return clienteApp?.AplicaMuestraPrecio;
        }
        public async Task GuardarClientesAplicacionesAsync(List<ClientesAplicacionesLocal> clientesAplicaciones)
        {
            await _database.DeleteAllAsync<ClientesAplicacionesLocal>();
            await _database.InsertAllAsync(clientesAplicaciones);
        }
        public async Task<List<ClientesAplicacionesLocal>> ObtenerClientesAplicacionesAsync()
        {
            return await _database.Table<ClientesAplicacionesLocal>().ToListAsync();
        }
        public async Task GuardarFacturacionAsync(List<FacturacionLocal> facturacion)
        {
            await _database.DeleteAllAsync<FacturacionLocal>();
            await _database.InsertAllAsync(facturacion); // <-- Usa InsertAllAsync para listas
        }
        public async Task<List<FacturacionLocal>> ObtenerFacturacionesAsync()
        {
            return await _database.Table<FacturacionLocal>().ToListAsync();
        }
        public async Task InsertarTicketDetalleAsync(TicketDetalleLocal detalle)
        {
            await _database.InsertAsync(detalle);
        }

        public async Task<List<TicketDetalleLocal>> ObtenerDetallesPorTicketAsync(Guid idTicket)
        {
            return await _database.Table<TicketDetalleLocal>()
                .Where(d => d.IdTicket == idTicket) // ✅ Buscar por ticket real
                .ToListAsync();
        }


        public async Task InsertarTicketAsync(TicketDetalleLocal ticket)
        {
            await _database.InsertAsync(ticket);
        }

        public async Task<List<TicketDetalleLocal>> ObtenerTicketsAsync()
        {
            return await _database.Table<TicketDetalleLocal>().ToListAsync();
        }

        public async Task ActualizarTicketAsync(TicketDetalleLocal ticket)
        {
            await _database.UpdateAsync(ticket);
        }
        public async Task GuardarRutaAsync(RutaLocal ruta)
        {
            await _database.DeleteAllAsync<RutaLocal>();
            await _database.InsertAsync(ruta);
        }

        public async Task<string?> ObtenerDescripcionRutaAsync()
        {
            var ruta = await ObtenerRutaAsync();
            return ruta?.Descripcion;
        }

        public async Task<Guid?> ObtenerIdRutaAsync()
        {
            var ruta = await ObtenerRutaAsync();
            return ruta?.Id;
        }

        // Método para obtener la ruta guardada
        public async Task<RutaLocal?> ObtenerRutaAsync()
        {
            return await _database.Table<RutaLocal>().FirstOrDefaultAsync();
        }
        public async Task GuardarEmpleadosAsync(List<EmpleadoLocal> empleados)
        {
            await _database.DeleteAllAsync<EmpleadoLocal>();
            await _database.InsertAllAsync(empleados);
        }

        public async Task<List<EmpleadoLocal>> ObtenerEmpleadosAsync()
        {
            return await _database.Table<EmpleadoLocal>().ToListAsync();
        }

        public async Task GuardarClientesAsync(List<ClienteLocal> clientes)
        {
            await _database.DeleteAllAsync<ClienteLocal>();
            await _database.InsertAllAsync(clientes);
        }

        public async Task<List<ClienteLocal>> ObtenerClientesAsync(Guid idrutaLocal)
        {
            return await _database.Table<ClienteLocal>().ToListAsync();
        }

        public async Task GuardarProductosAsync(List<ProductoLocal> productos)
        {
            await _database.DeleteAllAsync<ProductoLocal>();
            await _database.InsertAllAsync(productos);
        }

        public async Task<List<ProductoLocal>> ObtenerProductosAsync()
        {
            return await _database.Table<ProductoLocal>().ToListAsync();
        }

        public async Task<List<FormaLocal>> ObtenerFormasAsync()
        {
            return await _database.Table<FormaLocal>().ToListAsync();
        }
        public async Task GuardarFormasAsync(List<FormaLocal> formas)
        {
            await _database.DeleteAllAsync<FormaLocal>();
            await _database.InsertAllAsync(formas);
        }

        public async Task GuardarCondicionesAsync(List<CondicionLocal> condiciones)
        {
            await _database.DeleteAllAsync<CondicionLocal>();
            await _database.InsertAllAsync(condiciones);
        }

        public async Task<List<CondicionLocal>> ObtenerCondicionesAsync()
        {
            return await _database.Table<CondicionLocal>().ToListAsync();
        }

        public async Task GuardarPreciosGeneralesAsync(List<PreciosGeneralesLocal> precios)
        {
            await _database.DeleteAllAsync<PreciosGeneralesLocal>();
            await _database.InsertAllAsync(precios);
        }

        public async Task<List<PreciosGeneralesLocal>> ObtenerPreciosAsync()
        {
            return await _database.Table<PreciosGeneralesLocal>().ToListAsync();
        }

        public async Task<List<PreciosGeneralesLocal>> ObtenerPreciosGeneralesAsync()
        {
            return await _database.Table<PreciosGeneralesLocal>().ToListAsync();
        }
        public async Task<List<PreciosPreferencialesLocal>> ObtenerPreciosPreferencialesPorClienteAsync(Guid idClienteAsociado)
        {
            return await _database.Table<PreciosPreferencialesLocal>()
                .Where(p => p.IdClienteAsociado == idClienteAsociado)
                .ToListAsync();
        }
        public async Task GuardarPreciosPreferencialesAsync(List<PreciosPreferencialesLocal> preciosPref)
        {
            await _database.DeleteAllAsync<PreciosPreferencialesLocal>();
            await _database.InsertAllAsync(preciosPref);
        }

        public async Task<List<PreciosPreferencialesLocal>> ObtenerPreciosPreferencialesAsync()
        {
            return await _database.Table<PreciosPreferencialesLocal>().ToListAsync();
        }

        public async Task GuardarVentaAsync(VentaGeneralLocal venta)
        {
            try
            {
                await _database.InsertAsync(venta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al insertar VentaGeneral: {ex.Message}");
                throw;
            }
        }
        public async Task<List<VentaGeneralLocal>> ObtenerVentasAsync()
        {
            return await _database.Table<VentaGeneralLocal>().ToListAsync();
        }

        public async Task<List<VentaGeneralLocal>> ObtenerVentasNoSincronizadasAsync()
        {
            return await _database.Table<VentaGeneralLocal>()
                .Where(v => v.Sincronizado == false)
                .ToListAsync();
        }

        #region Ventas

        public async Task<VentaGeneralLocal> ObtenerVentaGeneralActiva()
        {
            var hoy = DateTime.Now.Date;
            var mañana = hoy.AddDays(1);

            return await _database.Table<VentaGeneralLocal>()
                .Where(v => v.Sincronizado == false &&
                            v.Fecha >= hoy &&
                            v.Fecha < mañana)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> GuardarVentaGeneral(VentaGeneralLocal venta)
        {
            var hoy = DateTime.Now.Date;
            var mañana = hoy.AddDays(1);

            bool existe = true;
            var ventaExistente = await _database.Table<VentaGeneralLocal>()
                .Where(v => v.Sincronizado == false &&
                            v.Fecha >= hoy &&
                            v.Fecha < mañana)
                .FirstOrDefaultAsync();

            if (ventaExistente == null)
            {
                await _database.InsertAsync(venta);
                existe = false;
            }
            return existe;
        }

        //Metodo para borrar la venta general activa    
        public async Task<bool> BorrarVentaGeneralActiva()
        {
            var venta = await ObtenerVentaGeneralActiva();
            if (venta != null)
            {
                await _database.DeleteAsync(venta);
                return true;
            }
            return false;
        }
        public async Task InsertarVentaDetalleAsync(VentaDetalleLocal detalle)
        {
            await _database.InsertAsync(detalle);
        }
        public async Task<List<VentaDetalleLocal>> ObtenerDetallesAsync()
        {
            return await _database.Table<VentaDetalleLocal>().ToListAsync();
        }

        public async Task<List<VentaDetalleLocal>> ObtenerDetallesPorVentaGeneralAsync(Guid idVentaGeneral)
        {
            return await _database.Table<VentaDetalleLocal>()
                .Where(d => d.IdVentaGeneral == idVentaGeneral)
                .ToListAsync();
        }

        public async Task ActualizarVentaAsync(VentaGeneralLocal venta)
        {
            await _database.UpdateAsync(venta);
        }

        #endregion
        public async Task<List<ValoresAppVentaDetalleLocal>> ObtenerValoresAppVentaDetalleAsync()
        {
            return await _database.Table<ValoresAppVentaDetalleLocal>().ToListAsync();
        }
        public async Task<int> ObtenerUltimoValorFolioVentaAsync()
        {
            var ultimo = await _database.Table<ValoresAppVentaDetalleLocal>()
                .OrderByDescending(x => x.ValorFolioVenta)
                .FirstOrDefaultAsync();
            return ultimo?.ValorFolioVenta ?? 0;
        }

        public async Task ImprimirVentasDebugAsync()
        {
            var ventas = await _database.Table<VentaGeneralLocal>().ToListAsync();
            System.Diagnostics.Debug.WriteLine("----- VENTAS EN BD -----");
            foreach (var v in ventas)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Id: {v.IdVentaGeneral}, Sincronizado: {v.Sincronizado}, Fecha: {v.Fecha:yyyy-MM-dd HH:mm:ss}, Ruta: {v.IdRuta}, Vuelta: {v.Vuelta}, IdStatusVenta: {v.IdStatusVenta}");
            }
            System.Diagnostics.Debug.WriteLine($"Total ventas: {ventas.Count}");
            var ventasNoSync = ventas.Where(x => x.Sincronizado == false).ToList();
            System.Diagnostics.Debug.WriteLine($"Ventas no sincronizadas: {ventasNoSync.Count}");
        }
    }
}
