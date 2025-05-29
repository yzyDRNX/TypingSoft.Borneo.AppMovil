using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    public class RutaLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("Descripcion")]
        public string? Descripcion { get; set; }
    }
}
