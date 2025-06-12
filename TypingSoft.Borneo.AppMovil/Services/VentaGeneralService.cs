using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class VentaGeneralService : Helpers.HttpClientBase
    {
        public VentaGeneralService() : base("VentaGeneral/")
        {
        }
        #region Métodos
        public async Task<(HttpStatusCode StatusCode, Models.API.VentaGeneralResponse Respuesta)> ObtenerVentasGenerales(Guid idRuta) => await CallGetAsync<Models.API.VentaGeneralResponse>("VentaGeneral");
        #endregion
    }
}
