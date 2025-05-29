using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Clientes")]
    public class ClienteLocal
    {
        [PrimaryKey]
        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }

        [Column("IdCliente")]
        public Guid IdCliente { get; set; }
   
     
        [Column("Cliente")]
        public string? Cliente { get; set; }
    }
}
