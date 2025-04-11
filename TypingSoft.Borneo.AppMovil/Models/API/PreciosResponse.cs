using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    class PreciosResponse: ResponseBase
    {
        public PreciosResponse()
        {
            this.Data = new List<Precios>();
        }
        public List<Precios> Data { get; set; }

        public class Precios
        {
            public Guid IdProducto { get; set; }
            public string? Empleado { get; set; }
            public decimal Precio { get; set; }
        }
    }
}
