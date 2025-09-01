using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class CondicionesPagoLista
    {
        public CondicionesPagoLista()
        {

        }
        public Guid Id { get; set; }
        public Guid IdClienteAsociado { get; set; }
        public Guid IdCondicionPago { get; set; }
    }
}
