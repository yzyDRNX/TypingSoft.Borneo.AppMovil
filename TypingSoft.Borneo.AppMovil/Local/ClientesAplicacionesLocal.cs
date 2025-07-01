using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("ClientesAplicaciones")]
    public class ClienteAplicacionesLocal
    {

        [PrimaryKey]
        [Column("Id")]
        public Guid Id { get; set; } 

        [Column("IdClienteAsociado")]
        public Guid IdClienteAsociado { get; set; }
        [Column("AplicaAPP")]
        public bool AplicaAPP { get; set; }
        [Column("AplicaMuestraPrecio")]
        public bool AplicaMuestraPrecio { get; set; }
        [Column("AplicaComodato")]
        public bool AplicaComodato { get; set; }
        [Column("AplicaDescuentos")]
        public bool AplicaDescuentos { get; set; }
        [Column("AplicaFacturacion")]
        public bool AplicaFacturacion { get; set; }
        [Column("AplicaCobranza")]
        public bool AplicaCobranza { get; set; }
        [Column("AplicaVales")]
        public bool AplicaVales { get; set; }
        [Column("AplicaMultiRuta")]
        public bool AplicaMultiRuta { get; set; }

    }
}
