using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class VentaGeneralRequestDTO: ResponseBase
    {
        public Guid IdVentaGeneral { get; set; }
        public Guid IdRuta { get; set; }
        public int Vuelta { get; set; }
        public DateTime Fecha { get; set; }
        public Guid IdStatusVenta { get; set; }
    }
}
