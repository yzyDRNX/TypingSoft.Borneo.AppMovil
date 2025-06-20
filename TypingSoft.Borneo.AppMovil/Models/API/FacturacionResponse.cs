using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.Custom;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class FacturacionResponse
    {
        public FacturacionResponse()
        {
            this.Data = new List<Facturacion>();
        }
        public List<Facturacion> Data { get; set; }
        public class Facturacion 
        {
            public Guid Id { get; set; }
            public Guid IdAsociado { get; set; }

            public string? RazonSocial { get; set; }
            public string? Calle { get; set; }
            public string? NumeroExterior { get; set; }
            public string? NumeroInterior { get; set; }

            public string? Colonia { get; set; }
            public string? CP { get; set; }
            public string? Municipio { get; set; }
            public string? Estado { get; set; }
            public Guid IdFormapago { get; set; }
            public Guid IdMetodoPago { get; set; }
            public Guid IdUsoCFDI { get; set; }
        }
    }
}
