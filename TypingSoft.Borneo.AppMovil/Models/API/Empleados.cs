using SQLite;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    [Table("Empleados")]
    public class Empleados
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("Nombre")]
        public string ? Nombre { get; set; }
        [Column("ApellidoPaterno")]
        public string? ApellidoPaterno { get; set; }
        [Column("ApellidoMaterno")]
        public string? ApellidoMaterno { get; set; }

    }
}
