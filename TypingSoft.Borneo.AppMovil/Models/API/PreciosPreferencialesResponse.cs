using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class PreciosPreferencialesResponse : ResponseBase
    {
        public PreciosPreferencialesResponse()
        {
            this.Data = new List<PreciosPref>();
        }
        public List<PreciosPref> Data { get; set; }

        public class PreciosPref
        {
            public Guid IdProducto { get; set; }
            public string? Producto { get; set; }
            public decimal Precio { get; set; }
            public Guid IdClienteAsociado { get; set; } 
        }
    }
}
