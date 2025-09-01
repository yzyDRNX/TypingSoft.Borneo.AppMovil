using SQLite;
using System;

namespace Borneo.Local
{
    [Table("CondicionesPago")]
    public class CondicionPagoLocal
    {
        // PK natural (proporcionada por el backend)
        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }

        [Indexed]
        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }

        [Column("IdCondicionPago")]
        public Guid IdCondicionPago { get; set; }
    }
}
