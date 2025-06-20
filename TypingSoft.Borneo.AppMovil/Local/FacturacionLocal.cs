using SQLite;
namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Facturacion")]
    public class FacturacionLocal
    {

        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("IdAsociado")]
        public Guid IdAsociado { get; set; }
        [Column("RazonSocial")]
        public string? RazonSocial { get; set; }
        [Column("Calle")]
        public string? Calle { get; set; }
        [Column("NumeroExterior")]
        public string? NumeroExterior { get; set; }
        [Column("NumeroInterior")]
        public string? NumeroInterior { get; set; }
        [Column("Colonia")]
        public string? Colonia { get; set; }
        [Column("CP")]
        public string? CP { get; set; }
        [Column("Municipio")]
        public string? Municipio { get; set; }
        [Column("Estado")]
        public string? Estado { get; set; }
        [Column("IdFormaPago")]
        public Guid IdFormaPago { get; set; }
        [Column("IdMetodoPago")]
        public Guid IdMetodoPago { get; set; }
        [Column("IdUsoCFDI")]
        public Guid IdUsoCFDI { get; set; }
    }
}
