using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class VentaGeneralResponse
    {
        public VentaGeneralResponse()
        {
            this.Data = new List<VentaGeneral>();
        }
        public List<VentaGeneral> Data { get; set; }
        public class VentaGeneral
        {
            public Guid IdVentaGeneral { get; set; }
            public Guid IdRuta { get; set; }
            public int Vuelta { get; set; }
            public DateTime Fecha { get; set; }
            public Guid IdStatusVenta { get; set; }
        }

    }
}
