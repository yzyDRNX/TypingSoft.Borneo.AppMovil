using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;


namespace TypingSoft.Borneo.AppMovil.Services
{
    public class LocalDatabaseService
    {
        private SQLiteAsyncConnection _database;

        public LocalDatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Empleados>(); // Haz lo mismo con otras entidades como Clientes, etc.
        }

        public Task<List<Empleados>> ObtenerEmpleadosAsync()
        {
            return _database.Table<Empleados>().ToListAsync();
        }

        public Task InsertarEmpleadosAsync(List<Empleados> empleados)
        {
            return _database.InsertAllAsync(empleados);
        }

        public Task BorrarEmpleadosAsync()
        {
            return _database.DeleteAllAsync<Empleados>();
        }
    }

}
