using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class ClientesAplicacionesResponse : ResponseBase
    {
        public ClientesAplicacionesResponse()
        {
            this.Data = new List<ClientesAplicaciones>();
        }
        public List<ClientesAplicaciones> Data { get; set; }

        public class ClientesAplicaciones
        {
            public Guid Id { get; set; }
            public Guid IdClienteAsociado { get; set; }
            public bool AplicaAPP { get; set; }
            public bool AplicaMuestraPrecio { get; set; }
            public bool AplicaComodato { get; set; }
            public bool AplicaDescuentos { get; set; }
            public bool AplicaFacturacion { get; set; }
            public bool AplicaCobranza { get; set; }
            public bool AplicaVales { get; set; }
            public bool AplicaMultiRuta { get; set; }
        }
    }
}
