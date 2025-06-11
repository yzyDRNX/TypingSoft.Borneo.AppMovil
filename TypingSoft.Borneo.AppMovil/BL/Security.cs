using System.Net;
using TypingSoft.Borneo.AppMovil.Helpers;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class Security
    {
        private readonly Services.SeguridadService _seguridadService;

        public Security(Services.SeguridadService seguridadService)
            => _seguridadService = seguridadService;

        public async Task<(bool Autenticado, string Mensaje, RutaResponse.Rutas? Ruta)>
            AutenticarRuta(string ruta)
        {
            try
            {
                var (status, respuesta) = await _seguridadService.IniciarSesion(ruta);

                if (status == HttpStatusCode.OK)
                {
                    if (respuesta.Exito)
                    {
                        StaticSettings.FijarConfiguracion(StaticSettings.IdRuta,respuesta.Data.Id.ToString());
                        return (true, respuesta.Mensaje, respuesta.Data);
                    }
                    else
                    {
                        return (false, respuesta.Mensaje, null);
                    }
                }
                else
                {
                    return (false, $"HTTP {status}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, "Error al conectar: " + ex.Message, null);
            }
        }
    }
}
