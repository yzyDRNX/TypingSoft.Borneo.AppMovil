using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Formas")]
    public class FormaLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("IdForma")]
        public Guid IdForma { get; set; }
        [Column("Forma")]
        public string? Forma { get; set; }
    }
}
