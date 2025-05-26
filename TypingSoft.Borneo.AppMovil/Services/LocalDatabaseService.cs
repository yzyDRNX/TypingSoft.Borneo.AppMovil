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

        public async Task<List<ClienteLocal>> ObtenerClientesAsync()
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

        public async Task GuardarFormasAsync(List<FormaLocal> formas)
        {
            await _database.DeleteAllAsync<FormaLocal>();
            await _database.InsertAllAsync(formas);
        }

        public async Task<List<FormaLocal>> ObtenerFormasAsync()
        {
            return await _database.Table<FormaLocal>().ToListAsync();
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
