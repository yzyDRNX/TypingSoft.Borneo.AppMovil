using System.Net;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class CatalogosService : Helpers.HttpClientBase
    {
        #region constructor
        public CatalogosService()
        {

        }
        #endregion

        #region Metodos
        public async Task<(HttpStatusCode StatusCode, Models.API.EmpleadosResponse Respuesta)> ObtenerEmpleados() => await CallGetAsync<Models.API.EmpleadosResponse>("Movil/ObtenerEmpleados");

        #endregion
    }
}
