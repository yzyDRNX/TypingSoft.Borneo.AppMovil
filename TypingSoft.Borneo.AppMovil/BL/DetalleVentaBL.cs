using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class DetalleVentaBL
    {
        #region Constructor
        Services.VentaDetalleService DetalleVentaService;
        public DetalleVentaBL(Services.VentaDetalleService detalleVentaService)
        {
            this.DetalleVentaService = detalleVentaService;
        }
        #endregion
        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.VentaDetalleRequestDTO> Detalles)> ObtenerDetalles(Guid idVentaGeneral)
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var detallesLista = new List<Models.Custom.VentaDetalleRequestDTO>();
            try
            {
                var peticion = await this.DetalleVentaService.ObtenerDetalle(idVentaGeneral);
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var detalle in peticion.Respuesta.Data)
                    {
                        detallesLista.Add(new Models.Custom.VentaDetalleRequestDTO
                        {
                            IdVentaGeneral = detalle.IdVentaGeneral,
                            IdProducto = detalle.IdProducto,
                            Cantidad = detalle.Cantidad,
                            ImporteTotal = detalle.ImporteTotal,
                            IdClienteAsociado = detalle.IdClienteAsociado,
                            IdCondicionPago = detalle.IdCondicionPago,
                            IdFormaPago = detalle.IdFormaPago

                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                detallesLista = new();
            }
            return (exitoso, mensaje, detallesLista);
        }
        // En DetalleVentaBL
        public async Task<(bool Exitoso, string Mensaje)> ExportarDetalleVenta(VentaDetalleLocal detalle)
        {
            var dto = new Models.Custom.VentaDetalleRequestDTO
            {
                IdVentaGeneral = detalle.IdVentaGeneral,
                IdProducto = detalle.IdProducto,
                Cantidad = detalle.Cantidad,
                ImporteTotal = detalle.ImporteTotal,
                IdClienteAsociado = detalle.IdClienteAsociado,
                IdCondicionPago = detalle.IdCondicionPago,
                IdFormaPago = detalle.IdFormaPago
            };
            var resultado = await DetalleVentaService.CallPostAsync<Models.Custom.VentaDetalleRequestDTO, Models.API.VentaDetalleResponse>("VentaDetalle", dto);
            return (resultado.StatusCode == System.Net.HttpStatusCode.OK, "sin mensajes");
        }
    }
}
