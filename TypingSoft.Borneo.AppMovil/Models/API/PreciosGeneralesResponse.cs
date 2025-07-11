﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class PreciosGeneralesResponse: ResponseBase
    {
        public PreciosGeneralesResponse()
        {
            this.Data = new List<Precios>();
        }
        public List<Precios> Data { get; set; }

        public class Precios
        {
            public Guid IdProducto { get; set; }
            public string? Producto { get; set; }
            public decimal Precio { get; set; }
        }
    }
}
