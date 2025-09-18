using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.BL;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class SincronizacionVentasService
    {
        private readonly VentaGeneralBL _ventaGeneralBL;
        private readonly DetalleVentaBL _detalleVentaBL;
        private readonly LocalDatabaseService _localDb;

        public SincronizacionVentasService(
            VentaGeneralBL ventaGeneralBL,
            DetalleVentaBL detalleVentaBL,
            LocalDatabaseService localDb)
        {       
            _ventaGeneralBL = ventaGeneralBL;
            _detalleVentaBL = detalleVentaBL;
            _localDb = localDb;
        }

        public async Task SincronizarVentasYDetallesAsync()
        {
            // Trae todas las ventas no sincronizadas (pueden ser de varios días)
            var ventasNoSync = await _localDb.ObtenerVentasNoSincronizadasAsync();
            if (ventasNoSync == null || ventasNoSync.Count == 0)
                return;

            // Agrupar por día (Fecha.Date) para exportar solo UNA VentaGeneral por día
            var gruposPorDia = ventasNoSync
                .GroupBy(v => v.Fecha.Date)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var grupo in gruposPorDia)
            {
                var ventasDelDia = grupo.OrderBy(v => v.Fecha).ToList();
                var ventaMaster = ventasDelDia.First();

                // Consolidar: mover detalles de ventas duplicadas hacia la master y eliminar las duplicadas
                if (ventasDelDia.Count > 1)
                {
                    foreach (var ventaDuplicada in ventasDelDia.Skip(1))
                    {
                        var detallesDuplicados = await _localDb.ObtenerDetallesPorVentaGeneralAsync(ventaDuplicada.IdVentaGeneral);
                        foreach (var det in detallesDuplicados)
                        {
                            det.IdVentaGeneral = ventaMaster.IdVentaGeneral;
                            await _localDb.ActualizarVentaDetalleAsync(det);
                        }

                        // Eliminar la VentaGeneral duplicada para que no se exporte nunca
                        await _localDb.EliminarVentaGeneralAsync(ventaDuplicada.IdVentaGeneral);
                    }
                }

                // Recuperar todos los detalles válidos de la venta master (ya consolidados)
                var detallesMaster = await _localDb.ObtenerDetallesPorVentaGeneralAsync(ventaMaster.IdVentaGeneral);
                var detallesValidos = detallesMaster
                    .Where(d => d.IdProducto != Guid.Empty && d.Cantidad > 0)
                    .ToList();

                if (detallesValidos.Count == 0)
                {
                    // Si no hay detalles, marcar como sincronizada y continuar
                    ventaMaster.Sincronizado = true;
                    await _localDb.ActualizarVentaAsync(ventaMaster);
                    continue;
                }

                // Calcular siguiente vuelta del día específico y por ruta
                var fechaDelDia = grupo.Key;
                var siguienteVuelta = await _localDb.ObtenerSiguienteVueltaPorFechaYRutaAsync(fechaDelDia, ventaMaster.IdRuta);
                ventaMaster.Vuelta = siguienteVuelta;
                await _localDb.ActualizarVentaAsync(ventaMaster);

                // Exportar solo UNA VentaGeneral por ese día y luego sus detalles
                var (exitoVenta, _) = await _ventaGeneralBL.ExportarVentaGeneral(ventaMaster);
                if (!exitoVenta)
                    continue;

                bool allOk = true;
                foreach (var detalle in detallesValidos)
                {
                    var (exitoDetalle, _) = await _detalleVentaBL.ExportarDetalleVenta(detalle);
                    if (!exitoDetalle)
                        allOk = false;
                }

                if (allOk)
                {
                    ventaMaster.Sincronizado = true;
                    await _localDb.ActualizarVentaAsync(ventaMaster);
                }

                await _localDb.ImprimirVentasDebugAsync();
            }
        }
    }
}
