using SQLite;


namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("VentaDetalleLocal")]
    public class VentaDetalleLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public Guid IdVentaGeneral { get; set; }

        [Column("IdProducto")]
        public Guid IdProducto { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("ImporteTotal")]
        public decimal ImporteTotal { get; set; }

        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }

        [Column("IdCondicionPago")]
        public Guid IdCondicionPago { get; set; }

        [Column("IdFormaPago")]
        public Guid IdFormaPago { get; set; }
    }
}
