using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class CondicionPagoResponse : ResponseBase
    {
        public CondicionPagoResponse()
        {
            this.Data = new List<CondicionesPago>();
        }
        public List<CondicionesPago> Data { get; set; }

        public class CondicionesPago
        {
            public Guid Id { get; set; }
            public Guid IdClienteAsociado { get; set; }
            public Guid IdCondicionPago { get; set; }
        }
    }
}
