using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Models.API;
using System.Net;
using System.Linq;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class ValoresAppVentaDetalleService : Helpers.HttpClientBase
    {
        public ValoresAppVentaDetalleService() : base("Reparto/") { }

        // POST: Reparto/ObtenerFolio (envelope con lista; opcionalmente filtra por ruta)
        public async Task<(bool Exitoso, string Mensaje, ValoresAppVentaDetalleResponse? Data)>
            ObtenerFolioAsync(Guid? idRuta = null)
        {
            var req = new ValoresAppVentaDetalleDTO { IdRuta = idRuta ?? Guid.Empty };

            var resultado = await this.CallPostAsync<ValoresAppVentaDetalleDTO, CustomResponse<List<ValoresAppVentaDetalleResponse>>>
                ("ObtenerFolio", req);

            var ok = resultado.StatusCode == HttpStatusCode.OK && (resultado.Content?.Exito ?? false);
            var msg = resultado.Content?.Mensaje ?? resultado.StatusCode.ToString();
            var item = resultado.Content?.Data?.FirstOrDefault();

            return (ok, msg, item);
        }

        // POST: Reparto/UpdateValoresAppVentaDetalle
        public async Task<(bool Exitoso, string Mensaje)> UpdateValoresAppVentaDetalleAsync(ValoresAppVentaDetalleDTO dto)
        {
            var resultado = await this.CallPostAsync<ValoresAppVentaDetalleDTO, ResponseBase>("UpdateValoresAppVentaDetalle", dto);
            var ok = resultado.StatusCode == HttpStatusCode.OK && (resultado.Content?.Exito ?? false);
            var msg = resultado.Content?.Mensaje ?? resultado.StatusCode.ToString();
            return (ok, msg);
        }

        // Helper: actualizar SOLO folio y fecha (manteniendo IdRuta/Serie)
        public async Task<(bool Exitoso, string Mensaje)> UpdateSoloFolioAsync(ValoresAppVentaDetalleLocal local, int nuevoFolio)
        {
            var dto = new ValoresAppVentaDetalleDTO
            {
                Id = local.Id,
                IdRuta = local.IdRuta,                 // se mantiene igual
                SerieVentaDetalle = local.SerieVentaDetalle, // se mantiene igual
                ValorFolioVenta = nuevoFolio,
                UltimaActualicacion = null            // servidor asigna SYSUTCDATETIME()
            };
            return await UpdateValoresAppVentaDetalleAsync(dto);
        }
    }
}