using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class ValoresAppVentaDetalleBL
    {
        private readonly LocalDatabaseService _localDb;
        private readonly ValoresAppVentaDetalleService _remoteService;

        public ValoresAppVentaDetalleBL(LocalDatabaseService localDb, ValoresAppVentaDetalleService remoteService)
        {
            _localDb = localDb;
            _remoteService = remoteService;
        }

        public async Task<bool> ObtenerYGuardarFolioServidorAsync()
        {
            var (ok, msg, data) = await _remoteService.ObtenerFolioAsync();
            if (!ok || data == null) return false;

            var local = new ValoresAppVentaDetalleLocal
            {
                Id = data.Id,
                IdRuta = data.IdRuta,
                ValorFolioVenta = data.ValorFolioVenta,
                SerieVentaDetalle = data.SerieVentaDetalle ?? "S",
                UltimaActualizacion = data.UltimaActualicacion
            };

            await _localDb.GuardarOActualizarFolioAsync(local);
            return true;
        }

        // Siempre intenta subir el folio. Si es 0/inexistente, lo incrementa primero.
        public async Task<bool> ActualizarFolioServidorDesdeLocalAsync()
        {
            System.Diagnostics.Debug.WriteLine("[SYNC][UpdateFolio] Iniciando");

            var idRuta = await _localDb.ObtenerIdRutaAsync() ?? Guid.Empty;
            ValoresAppVentaDetalleLocal? local = null;
            if (idRuta != Guid.Empty)
                local = await _localDb.ObtenerFolioPorRutaAsync(idRuta);

            if (local == null)
            {
                var descargado = await ObtenerYGuardarFolioServidorAsync();
                if (descargado && idRuta != Guid.Empty)
                    local = await _localDb.ObtenerFolioPorRutaAsync(idRuta);
            }
            if (local == null)
            {
                local = new ValoresAppVentaDetalleLocal
                {
                    Id = Guid.Empty,
                    IdRuta = idRuta,
                    ValorFolioVenta = 0,    
                    SerieVentaDetalle = "S",
                    UltimaActualizacion = DateTime.UtcNow
                };
                await _localDb.InsertarValoresAppVentaDetalleAsync(local);
            }

            // Folio efectivo = lo que diga el registro vs lo que se asignó en detalles
            var maxDetalles = await _localDb.ObtenerFolioMaximoEnDetallesAsync();
            var folioEfectivo = Math.Max(local.ValorFolioVenta, maxDetalles);

            if (folioEfectivo <= 0)
            {
                // Asegura que nunca subimos 0
                folioEfectivo = await _localDb.ReservarIncrementarFolioAsync();
                local.ValorFolioVenta = folioEfectivo;
                await _localDb.GuardarOActualizarFolioAsync(local);
            }
            else if (local.ValorFolioVenta != folioEfectivo)
            {
                // Alinea el registro local al folio mayor
                local.ValorFolioVenta = folioEfectivo;
                local.UltimaActualizacion = DateTime.UtcNow;
                await _localDb.GuardarOActualizarFolioAsync(local);
            }

            System.Diagnostics.Debug.WriteLine($"[SYNC][UpdateFolio] Folio a subir: {folioEfectivo}");

            var (ok, msg) = await _remoteService.UpdateSoloFolioAsync(local, folioEfectivo);
            System.Diagnostics.Debug.WriteLine($"[SYNC][UpdateFolio] {(ok ? "OK" : "ERROR")} - {msg}");
            return ok;
        }

        public async Task SincronizarAsync() => await ActualizarFolioServidorDesdeLocalAsync();
    }
}
