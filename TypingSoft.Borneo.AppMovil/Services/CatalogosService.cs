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

        public async Task<(HttpStatusCode StatusCode, Models.API.ProductosResponse Respuesta)> ObtenerProductos() => await CallGetAsync<Models.API.ProductosResponse>("ObtenerProductos");

        public async Task<(HttpStatusCode StatusCode, Models.API.FormasResponse Respuesta)> ObtenerFormas() => await CallGetAsync<Models.API.FormasResponse>("ObtenerFormas");

        public async Task<(HttpStatusCode StatusCode, Models.API.CondicionesResponse Respuesta)> ObtenerCondiciones() => await CallGetAsync<Models.API.CondicionesResponse>("ObtenerCondiciones");

        public async Task<(HttpStatusCode StatusCode, Models.API.PreciosGeneralesResponse Respuesta)> ObtenerPreciosGenerales() => await CallGetAsync<Models.API.PreciosGeneralesResponse>($"ObtenerPreciosGenerales/");

        public async Task<(HttpStatusCode StatusCode, Models.API.PreciosPreferencialesResponse Respuesta)> ObtenerPreciosPreferenciales() => await CallGetAsync<Models.API.PreciosPreferencialesResponse>($"ObtenerPreciosPreferenciales/");

        public async Task<(HttpStatusCode StatusCode, Models.API.FacturacionResponse Respuesta)> ObtenerFacturacion() => await CallGetAsync<Models.API.FacturacionResponse>($"ObtenerFacturacion/");

        public async Task<(HttpStatusCode StatusCode, Models.API.ClientesAplicacionesResponse Respuesta)> ObtenerClientesAplicaciones() => await CallGetAsync<Models.API.ClientesAplicacionesResponse>($"ObtenerClientesAplicaciones/");

        // NUEVO: condiciones de pago por cliente
        public async Task<(HttpStatusCode StatusCode, Models.API.CondicionPagoResponse Respuesta)> ObtenerClientesCondiciones()
            => await CallGetAsync<Models.API.CondicionPagoResponse>($"ObtenerClientesCondiciones/");
        #endregion
    }
}
