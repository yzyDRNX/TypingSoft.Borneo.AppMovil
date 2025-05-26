using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class CondicionesResponse : ResponseBase
    {
        public CondicionesResponse()
        {
            this.Data = new List<Condiciones>();
        }
        public List<Condiciones> Data { get; set; }

        public class Condiciones
        {
            public Guid IdCondicion { get; set; }
            public string? Condicion { get; set; }
        }
    }
}
