using System.Net;
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
                        Helpers.Settings.UltimaRuta = respuesta.Data.Descripcion;
                        return (true, respuesta.Mensaje, respuesta.Data);
                    }
                    else
                        return (false, respuesta.Mensaje, null);
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



        public async Task<(bool Exitoso, string Mensaje)> ObtenerInformacionRuta()
        {
            if (string.IsNullOrWhiteSpace(Helpers.Settings.UltimaRuta))
                return (false, "No hay ruta definida. Asegúrate de autenticar primero.");

            try
            {
                var informacion = await this._seguridadService.InformacionRuta(Helpers.Settings.UltimaRuta);

                if (informacion.StatusCode == HttpStatusCode.OK)
                {
                    if (informacion.Respuesta.Exito)
                    {
                        Helpers.Settings.UltimaRuta = null;
                        return (true, string.Empty);
                    }
                    else
                    {
                        return (false, informacion.Respuesta.Mensaje);
                    }
                }
                else
                {
                    return (false, $"Error HTTP {(int)informacion.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (false, "Ocurrió un error en la petición: " + ex.Message);
            }
        }


    }
}
