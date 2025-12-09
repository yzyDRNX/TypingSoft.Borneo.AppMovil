using Borneo.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Local;

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
                            IdVentaGeneral = catalogo.IdVentaGeneral,
                            IdRuta = catalogo.IdRuta,
                            Vuelta = catalogo.Vuelta,
                            Fecha = catalogo.Fecha,
                            IdStatusVenta = catalogo.IdStatusVenta,

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
        // En VentaGeneralBL
        public async Task<(bool Exitoso, string Mensaje)> ExportarVentaGeneral(VentaGeneralLocal venta)
        {
            var dto = new Models.Custom.VentaGeneralRequestDTO
            {
                IdVentaGeneral = venta.IdVentaGeneral,
                IdRuta = venta.IdRuta,
                Vuelta = venta.Vuelta,
                Fecha = venta.Fecha,
                IdStatusVenta = venta.IdStatusVenta
            };
            var resultado = await this.VentaGeneralService.CallPostAsync<Models.Custom.VentaGeneralRequestDTO, Models.API.VentaGeneralResponse>("VentaGeneral", dto);
            return (resultado.StatusCode == System.Net.HttpStatusCode.OK, "sin mensajes");
        }


        public async Task<(bool Exitoso, string Mensaje)> GuardarEmpleadoVentaDiaria(VentaDiariaEmpleadoLocal venta)
        {
          
            var resultado = await this.VentaGeneralService.CallPostAsync<VentaDiariaEmpleadoLocal, Models.API.ResponseBase>("VentaDiariaEmpleado", venta);
            return (resultado.StatusCode == System.Net.HttpStatusCode.OK, "sin mensajes");
        }
    }
}
