using System.Net;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Models.Custom;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class SeguridadService : Helpers.HttpClientBase
    {
        public SeguridadService() : base("Seguridad/") { }

        public async Task<(HttpStatusCode StatusCode, RutaResponse Respuesta)>IniciarSesion(string ruta)=> await CallPostAsync<RutaRequest, RutaResponse>("IniciarSesion",new RutaRequest { Ruta = ruta });

        public async Task<(HttpStatusCode StatusCode, RutaResponse Respuesta)>InformacionRuta(string descripcion)=> await CallGetAsync<RutaResponse>($"IniciarSesion/{descripcion}");
    }
}
