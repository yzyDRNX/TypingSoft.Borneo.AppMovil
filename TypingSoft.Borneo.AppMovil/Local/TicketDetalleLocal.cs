using SQLite;
using System;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("TicketDetalleLocal")]
    public class TicketDetalleLocal
    {
        [PrimaryKey]
        public Guid Id { get; set; } // Clave primaria única para el detalle

        [Column("IdTicket")]
        public Guid IdTicket { get; set; } // Relación con el ticket principal

        [Column("IdCliente")]
        public Guid IdCliente { get; set; }

        [Column("Cliente")]
        public string Cliente { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("ImporteTotal")]
        public decimal ImporteTotal { get; set; }

        [Column("Empleado")]
        public string? Empleado { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }
    }
}