using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("PreciosGenerales")]
    public class PreciosGeneralesLocal
    {
        [PrimaryKey]
        [Column("IdProducto")]
        public Guid IdProducto { get; set; }

        [Column("Producto")]
        public string? Producto { get; set; }

        [Column("Precio")]
        public string? Precio { get; set; }

    }
}
