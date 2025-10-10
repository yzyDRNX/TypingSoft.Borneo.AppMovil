using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("PreciosPreferenciales")]
    public class PreciosPreferencialesLocal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Column("IdProducto")]
        public Guid IdProducto { get; set; }

        [Column("Producto")]
        public string? Producto { get; set; }

        [Column("Precio")]
        public string? Precio { get; set; }

        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }
    }
}
