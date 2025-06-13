using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public class DetalleVentaInput
    {
        public string NombreProducto { get; set; }
        public string NombreCondicion { get; set; }
        public string NombreForma { get; set; }
        public Guid IdClienteAsociado { get; set; }
        public int Cantidad { get; set; }
        public decimal ImporteTotal { get; set; }
    }

    public partial class DetalleVentaVM : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;

        public DetalleVentaVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;
        }

        [RelayCommand]
        public async Task InsertarDetalleVentaAsync(DetalleVentaInput input)
        {
            // Obtener IdProducto
            var productos = await _localDb.ObtenerProductosAsync();
            var producto = productos.FirstOrDefault(p => p.Producto == input.NombreProducto);
            if (producto == null)
            {
                // Manejar error: producto no encontrado
                return;
            }

            // Obtener IdCondicionPago
            var condiciones = await _localDb.ObtenerCondicionesAsync();
            var condicion = condiciones.FirstOrDefault(c => c.Condicion == input.NombreCondicion);
            if (condicion == null)
            {
                // Manejar error: condición no encontrada
                return;
            }

            // Obtener IdFormaPago
            var formas = await _localDb.ObtenerFormasAsync();
            var forma = formas.FirstOrDefault(f => f.Forma == input.NombreForma);
            if (forma == null)
            {
                // Manejar error: forma de pago no encontrada
                return;
            }

            // Obtener la venta general activa para asociar el detalle
            var ventaGeneral = await ObtenerVentaGeneralActiva();
            if (ventaGeneral != null)
            {
                // Crear el detalle de venta
                var detalle = new VentaDetalleLocal
                {
                    IdDetalle = Guid.NewGuid(),
                    IdVentaGeneral = ventaGeneral.IdVentaGeneral,
                    IdProducto = producto.Id,
                    Cantidad = input.Cantidad,
                    ImporteTotal = input.ImporteTotal,
                    IdClienteAsociado = input.IdClienteAsociado,
                    IdCondicionPago = condicion.IdCondicion,
                    IdFormaPago = forma.IdForma
                };

                // Insertar en la base de datos
                await InsertarVentaDetalleAsync(detalle);
            }
        }

        private async Task<VentaGeneralLocal> ObtenerVentaGeneralActiva()
        {
            return await _localDb.ObtenerVentaGeneralActiva();
        }

        private async Task InsertarVentaDetalleAsync(VentaDetalleLocal detalle)
        {
            await _localDb.InsertarVentaDetalleAsync(detalle);
        }
    }
}