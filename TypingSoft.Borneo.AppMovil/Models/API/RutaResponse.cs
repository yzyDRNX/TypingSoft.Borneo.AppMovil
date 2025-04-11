using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
    class RutaResponse: ResponseBase
    {
        public RutaResponse()
        {
            this.Data = new List<Rutas>();
        }
        public List<Rutas> Data { get; set; }

        public class Rutas
        {
            public Guid Id { get; set; }
            public string? Descripcion { get; set; }
        }
    }
}
