using System.Net;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class CatalogosService : Helpers.HttpClientBase
    {
        #region constructor
        public CatalogosService() : base("Catalogos/")
        {

        }
        #endregion

        #region Metodos
        public async Task<(HttpStatusCode StatusCode, Models.API.EmpleadosResponse Respuesta)> ObtenerEmpleados() => await CallGetAsync<Models.API.EmpleadosResponse>("ObtenerEmpleados");

        public async Task<(HttpStatusCode StatusCode, Models.API.ClientesResponse Respuesta)> ObtenerClientes(Guid idRuta) => await CallGetAsync<Models.API.ClientesResponse>($"ObtenerClientes/{idRuta}");

        public async Task<(HttpStatusCode StatusCode, Models.API.ProductosResponse Respuesta)> ObtenerProductos(Guid idRuta) => await CallGetAsync<Models.API.ProductosResponse>($"ObtenerProductos/{idRuta}");
        #endregion
    }
}
