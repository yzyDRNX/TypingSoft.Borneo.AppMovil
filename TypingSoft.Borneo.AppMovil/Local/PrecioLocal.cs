using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Precios")]
    public class PrecioLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("IdProducto")]
        public Guid IdProducto { get; set; }

        [Column("Producto")]
        public string? Producto { get; set; }

        [Column("Precio")]
        public string? Precio { get; set; }

    }
}
