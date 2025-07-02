using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("VentaGeneralLocal")]
    public class VentaGeneralLocal
    {
        [PrimaryKey]
        [Column("IdVentaGeneral")]
        public Guid IdVentaGeneral { get; set; }

        [Column("IdRuta")]
        public Guid IdRuta { get; set; }

        [Column("Vuelta")]
        public int Vuelta { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("IdStatusVenta")]
        public Guid IdStatusVenta { get; set; }

        [Column("Sincronizado")]
        public bool Sincronizado { get; set; }

    }
}