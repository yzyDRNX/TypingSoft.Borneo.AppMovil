using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class Security
    {
        #region Constructor
        Services.SeguridadService SeguridadService;
        public Security(Services.SeguridadService seguridadService)
        {
            this.SeguridadService = seguridadService;
        }
        #endregion

        #region Métodos


        public async Task<(bool Autenticado, string Mensaje)> AutenticarRuta(string Ruta)
        {
            var autenticado = false;
            var mensaje = "Ocurrió un error en la petición";
            try
            {
                var autenticacion = await this.SeguridadService.IniciarSesion(Ruta);
                if (autenticacion.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    autenticado = autenticacion.Respuesta.Exito;
                    Helpers.Settings.UltimaRuta = Ruta;

                    mensaje = string.Empty;
                    if (!autenticado)
                    {
                        mensaje = autenticacion.Respuesta.Mensaje;
                    }
                }
            }
            catch
            {
                mensaje = "Ocurrió un error en la petición";
            }
            return (autenticado, mensaje);
        }



        public async Task<(bool Exitoso, string Mensaje)> ObtenerInformacionRuta()
        {
            var validado = false;
            var mensaje = "Ocurrió un error en la petición";
            try
            {
                var informacion = await this.SeguridadService.InformacionRuta(Helpers.Settings.UltimaRuta);
                if (informacion.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    validado = informacion.Respuesta.Exito;
                    mensaje = string.Empty;
                    Helpers.Settings.UltimaRuta = string.Empty;
                    if (!validado)
                        mensaje = informacion.Respuesta.Mensaje;
                    else
                    {

                    }
                }
            }
            catch
            {
                mensaje = "Ocurrió un error en la petición";
            }
            return (validado, mensaje);
        }

        #endregion
    }
}
