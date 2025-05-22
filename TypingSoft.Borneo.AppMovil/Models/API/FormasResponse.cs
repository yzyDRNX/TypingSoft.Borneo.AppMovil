using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class FormasResponse : ResponseBase
    {
        public FormasResponse()
        {
            this.Data = new List<Formas>();
        }
        public List<Formas> Data { get; set; }

        public class Formas
        {
            public Guid IdForma { get; set; }
            public string? Forma { get; set; }
        }
    }
}