using System.Net;
using TypingSoft.Borneo.AppMovil.Models.Custom;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class SeguridadService : Helpers.HttpClientBase
    {
        #region Constructor
        public SeguridadService() : base("Seguridad/")
        {
        }
        #endregion        

        //public async Task<(HttpStatusCode StatusCode, Models.API.ResponseBase Respuesta)> ValidarRFC(string rfc) => await CallGetAsync<Models.API.ResponseBase>($"VerificarMaestro/{rfc}");



        public async Task<(HttpStatusCode StatusCode, Models.API.ResponseBase Respuesta)> IniciarSesion(string ruta) => await CallPostAsync<Models.Custom.RutaRequest, Models.API.CustomResponse<Rutas>>($"IniciarSesion",new Models.Custom.RutaRequest() { Ruta=ruta});

        public async Task<(HttpStatusCode StatusCode, Models.API.RutaResponse Respuesta)> InformacionRuta(string Descripcion) => await CallGetAsync<Models.API.RutaResponse>($"IniciarSesion/{Descripcion}");
    }
}
