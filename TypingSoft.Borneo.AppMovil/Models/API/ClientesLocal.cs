using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    [Table("Clientes")]
    public class ClientesLocal
    {
        [PrimaryKey]
        public Guid IdCliente { get; set; }

        public Guid IdClienteAsociado { get; set; }

        public string? Cliente { get; set; }
    }
}
