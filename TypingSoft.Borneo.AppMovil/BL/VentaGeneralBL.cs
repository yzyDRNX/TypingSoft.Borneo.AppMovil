using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class VentaGeneralBL
    {
        #region Constructor
        Services.VentaGeneralService VentaGeneralService;
        public VentaGeneralBL(Services.VentaGeneralService ventaGeneralService)
        {
            this.VentaGeneralService = ventaGeneralService;
        }
        #endregion
        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.VentaGeneralRequestDTO> VentasGenerales)> ObtenerVentasGenerales(Guid idRuta)
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var ventasGenerales = new List<Models.Custom.VentaGeneralRequestDTO>();
            try
            {
                var peticion = await this.VentaGeneralService.ObtenerVentasGenerales(idRuta);
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        ventasGenerales.Add(new Models.Custom.VentaGeneralRequestDTO
                        {
                            IdRuta = catalogo.IdRuta,
                            Vuelta = catalogo.Vuelta,
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                ventasGenerales = new();
            }
            return (exitoso, mensaje, ventasGenerales);
        }
    }
}
