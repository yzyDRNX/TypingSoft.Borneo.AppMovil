using SQLite;
using System.IO;
using TypingSoft.Borneo.AppMovil.Local;

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
            _database.CreateTableAsync<PrecioLocal>().Wait();
            _database.CreateTableAsync<RutaLocal>().Wait();
            _database.CreateTableAsync<SeleccionTemporal>().Wait();
            _database.CreateTableAsync<VentaGeneralLocal>().Wait(); // ← Nuevo
        }


        public async Task GuardarRutaAsync(RutaLocal ruta)
        {
            // Si solo quieres una ruta, puedes limpiar antes
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

        public async Task GuardarPreciosAsync(List<PrecioLocal> precios)
        {
            await _database.DeleteAllAsync<PrecioLocal>();
            await _database.InsertAllAsync(precios);
        }

        public async Task<List<PrecioLocal>> ObtenerPreciosAsync()
        {
            return await _database.Table<PrecioLocal>().ToListAsync();
        }
        public async Task GuardarClienteTemporalAsync(string cliente)
        {
            var seleccion = await _database.Table<SeleccionTemporal>().FirstOrDefaultAsync();

            if (seleccion == null)
            {
                seleccion = new SeleccionTemporal { Cliente = cliente };
                await _database.InsertAsync(seleccion);
            }
            else
            {
                seleccion.Cliente = cliente;
                await _database.UpdateAsync(seleccion);
            }
        }

        public async Task GuardarProductoTemporalAsync(string producto)
        {
            var seleccion = await _database.Table<SeleccionTemporal>().FirstOrDefaultAsync();

            if (seleccion == null)
            {
                seleccion = new SeleccionTemporal { Producto = producto };
                await _database.InsertAsync(seleccion);
            }
            else
            {
                seleccion.Producto = producto;
                await _database.UpdateAsync(seleccion);
            }
        }

        public async Task<SeleccionTemporal?> ObtenerSeleccionTemporalAsync()
        {
            return await _database.Table<SeleccionTemporal>().FirstOrDefaultAsync();
        }

        public async Task LimpiarSeleccionTemporalAsync()
        {
            await _database.DeleteAllAsync<SeleccionTemporal>();
        }
        public async Task GuardarSeleccionTemporalAsync(List<SeleccionTemporal> seleccion)
        {
            await _database.DeleteAllAsync<SeleccionTemporal>(); // Limpia la selección anterior
            await _database.InsertAllAsync(seleccion);
        }

        public async Task GuardarVentaAsync(VentaGeneralLocal venta)
        {
            await _database.InsertAsync(venta);
        }
        public async Task<List<VentaGeneralLocal>> ObtenerVentasAsync()
        {
            return await _database.Table<VentaGeneralLocal>().ToListAsync();
        }

    }
}
