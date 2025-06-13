using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("VentaDetalleLocal")]
    public class VentaGeneralLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("IdRuta")]
        public Guid IdRuta { get; set; }

        [Column("Vuelta")]
        public int Vuelta { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Sincronizado")]
        public bool Sincronizado { get; set; }
    }
}
