using System;
using System.Collections.Generic;
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

        // Inserta un registro en la base local
        public async Task InsertarLocalAsync(ValoresAppVentaDetalleLocal detalle)
        {
            await _localDb.InsertarValoresAppVentaDetalleAsync(detalle);
        }

        // Obtiene todos los registros locales
        public async Task<List<ValoresAppVentaDetalleLocal>> ObtenerLocalesAsync()
        {
            return await _localDb.ObtenerValoresAppVentaDetalleAsync();
        }

        // Sincroniza todos los registros locales con el servidor
        public async Task SincronizarAsync()
        {
            var registros = await ObtenerLocalesAsync();
            foreach (var registro in registros)
            {
                var (exitoso, mensaje) = await _remoteService.ExportarValoresAppVentaDetalle(registro);
                // Aquí podrías marcar el registro como sincronizado si lo necesitas
            }
        }
    }
}
