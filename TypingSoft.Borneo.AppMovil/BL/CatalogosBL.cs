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
        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.EmpleadosLista> Empleados)> ObtenerEmpleados()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var empleadosLista = new List<Models.Custom.EmpleadosLista>();
            try
            {
                var peticion = await this.CatalogosService.ObtenerEmpleados();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        empleadosLista.Add(new Models.Custom.EmpleadosLista
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
                empleadosLista = new();
            }
            return (exitoso, mensaje, empleadosLista);
        }


    }
}
