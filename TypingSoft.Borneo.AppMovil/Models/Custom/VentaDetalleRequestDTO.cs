using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class VentaDetalleRequestDTO : ResponseBase
    {
        public Guid IdVentaDetalle { get; set; }
        public Guid IdVentaGeneral { get; set; }
        public Guid IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal ImporteTotal { get; set; }
        public Guid IdClienteAsociado { get; set; }
        public Guid IdCondicionPago { get; set; }
        public Guid IdFormaPago { get; set; }


    }
}
