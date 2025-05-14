using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class ClientesResponse: ResponseBase
    {
        public ClientesResponse()
        {
            this.Data = new List<Clientes>();
        }
        public List<Clientes> Data { get; set; }

        public class Clientes
        {
            public Guid IdCliente { get; set; }
            public Guid IdClienteAsociado { get; set; } 
            public string? Cliente { get; set; }
        }
    }
}
