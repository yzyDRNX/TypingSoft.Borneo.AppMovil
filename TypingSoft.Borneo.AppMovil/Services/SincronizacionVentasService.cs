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
            var ventas = await _localDb.ObtenerVentasNoSincronizadasAsync();

            foreach (var venta in ventas)
            {
                var (exitoVenta, mensajeVenta) = await _ventaGeneralBL.ExportarVentaGeneral(venta);

                if (exitoVenta)
                {
                    var detalles = await _localDb.ObtenerDetallesPorVentaGeneralAsync(venta.IdVentaGeneral);

                    // Filtra detalles válidos
                    var detallesValidos = detalles
                        .Where(d => d.IdProducto != Guid.Empty && d.Cantidad > 0)
                        .ToList();

                    foreach (var detalle in detallesValidos)
                    {
                        var (exitoDetalle, mensajeDetalle) = await _detalleVentaBL.ExportarDetalleVenta(detalle);
                    }

                    venta.Sincronizado = true;
                    await _localDb.ActualizarVentaAsync(venta);

                    // Imprime el estado después de actualizar
                    await _localDb.ImprimirVentasDebugAsync();
                }
            }
        }
    }
}
