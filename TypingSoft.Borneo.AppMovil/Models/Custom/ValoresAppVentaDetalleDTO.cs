using System;
using Newtonsoft.Json;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class ValoresAppVentaDetalleDTO
    {
        public Guid Id { get; set; }
        public Guid IdRuta { get; set; }
        public int ValorFolioVenta { get; set; }
        public string? SerieVentaDetalle { get; set; }

        // Enviar null para que el SP use SYSUTCDATETIME()
        [JsonProperty("UltimaActualizacion", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? UltimaActualicacion { get; set; }
    }
}
