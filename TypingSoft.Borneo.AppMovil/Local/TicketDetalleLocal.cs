using SQLite;
using System;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("TicketDetalleLocal")]
    public class TicketDetalleLocal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid TicketId { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
        public Guid IdCliente { get; set; }
    }
}