using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Clientes")]
    public class ClienteLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("IdCliente")]
        public Guid IdCliente { get; set; }
        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }
        [Column("Cliente")]
        public string? Cliente { get; set; }
    }
}
