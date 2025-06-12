using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class VentaDetalleResponse
    {
        public VentaDetalleResponse()
        {
            this.Data = new List<VentaDetalle>();
        }
        public List<VentaDetalle> Data { get; set; }
        public class VentaDetalle
        {
            public Guid IdVentaGeneral { get; set; }
            public Guid IdProducto { get; set; }
            public int Cantidad { get; set; }
            public decimal ImporteTotal { get; set; }
            public Guid IdClienteAsociado { get; set; }
            public Guid IdCondicionPago { get; set; }
            public Guid IdFormaPago { get; set; }
        }

    }
}
