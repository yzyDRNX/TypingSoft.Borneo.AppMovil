using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class ValoresAppVentaDetalleDTO
    {
        public Guid Id { get; set; }
        public Guid IdRuta { get; set; }
        public int ValorFolioVenta { get; set; }
        public string? SerieVentaDetalle { get; set; }
        public DateTime UltimaActualicacion { get; set; }
    }
}
