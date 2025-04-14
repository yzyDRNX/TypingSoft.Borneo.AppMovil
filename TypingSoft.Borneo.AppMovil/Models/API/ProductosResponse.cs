using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    class ProductosResponse : ResponseBase
    {
        public ProductosResponse()
        {
            this.Data = new List<Productos>();
        }
        public List<Productos> Data { get; set; }

        public class Productos
        {
            public Guid Id { get; set; }
            public string? Producto { get; set; }
        }
    }
}
