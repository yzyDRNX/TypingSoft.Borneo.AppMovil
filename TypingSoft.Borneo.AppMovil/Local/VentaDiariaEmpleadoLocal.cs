using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borneo.Local
{
    [Table("VentaDiariaEmpleadoLocal")]
    public class VentaDiariaEmpleadoLocal
    {
        [PrimaryKey]  
        public Guid Id { get; set; }
        public Guid IdEmpleado { get; set; }
        public Guid IdVentaGeneral { get; set; }
        public bool Sincronizado { get; set; }
    }
}
