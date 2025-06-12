using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class VentaDetalleService : Helpers.HttpClientBase
    {
        #region Constructor
        public VentaDetalleService() : base("VentaDetalle/")
        {
        }
        #endregion
        #region Métodos
        public async Task<(HttpStatusCode StatusCode, Models.API.VentaDetalleResponse Respuesta)> ObtenerDetalle(Guid idVentaGeneral) => await CallGetAsync<Models.API.VentaDetalleResponse>("VentaDetalle");
        #endregion

    }
}
