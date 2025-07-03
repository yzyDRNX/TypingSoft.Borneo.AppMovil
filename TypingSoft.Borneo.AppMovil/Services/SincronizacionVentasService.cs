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
            // Obtén las ventas no sincronizadas
            var ventas = await _localDb.ObtenerVentasNoSincronizadasAsync();

            foreach (var venta in ventas)
            {
                var (exitoVenta, mensajeVenta) = await _ventaGeneralBL.ExportarVentaGeneral(venta);

                if (exitoVenta)
                {
                    // Obtén los detalles de la venta
                    var detalles = await _localDb.ObtenerDetallesPorVentaGeneralAsync(venta.IdVentaGeneral);

                    foreach (var detalle in detalles)
                    {
                        var (exitoDetalle, mensajeDetalle) = await _detalleVentaBL.ExportarDetalleVenta(detalle);
                        // Aquí puedes manejar el resultado de cada detalle si lo necesitas
                    }

                    // Marca la venta como sincronizada en la base local
                    venta.Sincronizado = true;
                    await _localDb.ActualizarVentaAsync(venta);
                }
                else
                {
                    // Maneja el error de exportación de la venta si es necesario
                }
            }
        }
    }
}
