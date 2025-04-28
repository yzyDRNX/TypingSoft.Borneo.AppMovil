using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class LocalDBService
    {
        private const string BorneoDB = "Borneo.db";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDBService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, BorneoDB));
            _connection.CreateTableAsync<Models.API.Empleados>();
        }

        public async Task<List<Models.API.Empleados>> GetEmpleados()
        {
            return await _connection.Table<Models.API.Empleados>().ToListAsync();
        }

        public async Task<Models.API.Empleados> GetEmpleado(Guid id)
        {
            return await _connection.Table<Models.API.Empleados>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Models.API.Empleados empleado)
        {
            await _connection.InsertAsync(empleado);
        }

        public async Task Update(Models.API.Empleados empleado)
        {
            await _connection.UpdateAsync(empleado);
        }

        public async Task Delete(Models.API.Empleados empleado)
        {
            await _connection.DeleteAsync(empleado);
        }
    }

}
