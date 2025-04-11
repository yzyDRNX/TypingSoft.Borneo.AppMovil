using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.BL
{
    public class CatalogosBL
    {

        #region Constructor
        Services.CatalogosService CatalogosService;
        public CatalogosBL(Services.CatalogosService catalogosService)
        {
            this.CatalogosService = catalogosService;
        }
        #endregion
        public async Task<(bool Exitoso, string Mensaje, List<Models.API.EmpleadosResponse> Emplados)> ObtenerEmpleados()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var empleadosLista = new List<Models.API.EmpleadosResponse>();
            try
            {
                var peticion = await this.CatalogosService.ObtenerEmpleados();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        empleadosLista.Add(new Models.API.EmpleadosResponse
                        {
                            Id = catalogo.Id,
                            Empleado = catalogo.Empleado,
                          


                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                ahorrosLista = new();
            }
            return (exitoso, mensaje, ahorrosLista);
        }
        public async Task<(bool Exitoso, string Mensaje)> EnviarCorreoEstadoCuentaAhorro()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";

            try
            {
                Guid IdEmpleo = Helpers.StaticSettings.ObtenerValor(Helpers.StaticSettings.IdEmpleo);
                string correo = Helpers.StaticSettings.ObtenerValor<string>(Helpers.StaticSettings.CorreoElectronico);
                var peticion = await this.AhorrosService.EnviarEstadoCuentaAhorro(IdEmpleo, correo);
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    return (exitoso, mensaje);
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";

            }
            return (exitoso, mensaje);
        }

    }
}
