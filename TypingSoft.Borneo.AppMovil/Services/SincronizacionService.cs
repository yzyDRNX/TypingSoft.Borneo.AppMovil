// Services/SincronizacionService.cs
using TypingSoft.Borneo.AppMovil.Models.API;
using SQLite;
using Newtonsoft.Json;

public class SincronizacionService
{
    private readonly HttpClient _http;
    private readonly SQLiteAsyncConnection _db;

    public SincronizacionService()
    {
        _http = new HttpClient();
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "borneo.db3");
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<ClientesLocal>().Wait();
    }

    public async Task DescargarClientesDesdeApi(Guid idRuta)
    {
        try
        {
            var url = $"https://tuservidor.com/api/Catalogos/ObtenerClientes?IdRuta={idRuta}";
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ClientesResponse>(json);

                foreach (var c in resultado.Data)
                {
                    var clienteLocal = new ClientesLocal
                    {
                        IdCliente = c.IdCliente,
                        IdClienteAsociado = c.IdClienteAsociado,
                        Cliente = c.Cliente
                    };

                    await _db.InsertOrReplaceAsync(clienteLocal);
                }

                Console.WriteLine("Clientes sincronizados correctamente.");
            }
            else
            {
                Console.WriteLine($"Error al obtener clientes: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task<List<ClientesLocal>> ObtenerClientesLocales()
    {
        return await _db.Table<ClientesLocal>().ToListAsync();
    }
}
