using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Productos")]
    public class ProductoLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("Producto")]
        public string? Producto { get; set; }
    }
}
