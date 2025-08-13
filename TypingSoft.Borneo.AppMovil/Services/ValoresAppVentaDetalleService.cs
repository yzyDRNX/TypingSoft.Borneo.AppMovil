using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Models.API;
using System.Net;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class ValoresAppVentaDetalleService : Helpers.HttpClientBase
    {
        public ValoresAppVentaDetalleService() : base("Reparto/") { }

        public async Task<(bool Exitoso, string Mensaje)> ExportarValoresAppVentaDetalle(ValoresAppVentaDetalleLocal local)
        {
            var dto = new ValoresAppVentaDetalleDTO
            {
                Id = local.Id,
                IdRuta = local.IdRuta,
                ValorFolioVenta = local.ValorFolioVenta,
                SerieVentaDetalle = local.SerieVentaDetalle,
                UltimaActualicacion = local.UltimaActualizacion
            };

            var resultado = await this.CallPostAsync<ValoresAppVentaDetalleDTO, ValoresAppVentaDetalleResponse>("ValoresAppVentaDetalle", dto);

            return (resultado.StatusCode == HttpStatusCode.OK, resultado.StatusCode.ToString());
        }

        public async Task SincronizarValoresAppVentaDetalleAsync(LocalDatabaseService localDb)
        {
            var registrosLocales = await localDb.ObtenerValoresAppVentaDetalleAsync(); // Debes implementar este método en LocalDatabaseService

            foreach (var registro in registrosLocales)
            {
                var (exitoso, mensaje) = await ExportarValoresAppVentaDetalle(registro);
                // Si exitoso, puedes marcar el registro como sincronizado si lo deseas
            }
        }
    }
}