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
        }

        public async Task GuardarRutaAsync(RutaLocal ruta)
        {
            // Si solo quieres una ruta, puedes limpiar antes
            await _database.DeleteAllAsync<RutaLocal>();
            await _database.InsertAsync(ruta);
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
            if (clientes == null || clientes.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No hay clientes para guardar.");
                return;
            }
            await _database.DeleteAllAsync<ClienteLocal>();
            await _database.InsertAllAsync(clientes);
            System.Diagnostics.Debug.WriteLine($"Clientes guardados en SQLite: {clientes.Count}");
        }



        public async Task<List<ClienteLocal>> ObtenerClientesAsync()
        {
            var lista = await _database.Table<ClienteLocal>().ToListAsync();
            System.Diagnostics.Debug.WriteLine($"Clientes leídos de SQLite: {lista.Count}");
            return lista;
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
    }
}
