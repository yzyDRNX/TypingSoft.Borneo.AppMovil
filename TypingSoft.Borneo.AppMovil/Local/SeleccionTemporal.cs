using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("SeleccionTemporal")]
    public class SeleccionTemporal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("Descripcion")]
        public string? Descripcion { get; set; }

        [Column("Cliente")]
        public string? Cliente { get; set; }

        [Column("Producto")]
        public string? Producto { get; set; }

        [Column("Cantidad")]
        public string? Cantidad { get; set; }
    }


}
