using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Local; // <-- Agrega este using

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class VentaGeneralService : Helpers.HttpClientBase
    {
        public VentaGeneralService() : base("Reparto/")
        {
        }
        #region Métodos
        public async Task<(HttpStatusCode StatusCode, Models.API.VentaGeneralResponse Respuesta)> ObtenerVentasGenerales(Guid idRuta) => await CallGetAsync<Models.API.VentaGeneralResponse>("VentaGeneral");

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

            System.Diagnostics.Debug.WriteLine($"[SYNC] Enviando VentaGeneral: {dto.IdVentaGeneral}");

            // Usa 'this.' para llamar al método de instancia
            var resultado = await this.CallPostAsync<Models.Custom.VentaGeneralRequestDTO, Models.API.VentaGeneralResponse>("VentaGeneral", dto);

            System.Diagnostics.Debug.WriteLine($"[SYNC] Respuesta VentaGeneral: {resultado.StatusCode}");

            return (resultado.StatusCode == System.Net.HttpStatusCode.OK, "sin mensajes");
        }
        #endregion
    }
}
