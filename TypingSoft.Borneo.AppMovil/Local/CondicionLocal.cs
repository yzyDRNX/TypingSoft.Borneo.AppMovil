using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Condiciones")]
    public class CondicionLocal
    {
        [PrimaryKey]
        [Column("IdCondicion")]
        public Guid IdCondicion { get; set; }

        [Column("Condicion")]
        public string? Condicion { get; set; }
    }
}
