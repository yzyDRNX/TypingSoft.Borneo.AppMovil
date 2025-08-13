using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("ValoresAppVentaDetalle")]
    public class ValoresAppVentaDetalleLocal
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("IdRuta")]
        public Guid IdRuta { get; set; }

        [Column("ValorFolioVenta")]
        public int ValorFolioVenta { get; set; }

        [Column("SerieVentaDetalle")]
        public string SerieVentaDetalle { get; set; }

        [Column("UltimaActualizacion")]
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
    }
}
