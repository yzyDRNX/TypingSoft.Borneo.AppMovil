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
                            Empleado = catalogo.Empleado 
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


        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.ClientesLista> Clientes)> ObtenerClientes(Guid idRuta)
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var clientesLista = new List<Models.Custom.ClientesLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerClientes(idRuta);
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        clientesLista.Add(new Models.Custom.ClientesLista
                        {
                            IdCliente = catalogo.IdCliente,
                            IdClienteAsociado = catalogo.IdClienteAsociado, 
                            Cliente = catalogo.Cliente
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                clientesLista = new List<Models.Custom.ClientesLista>();
            }

            return (exitoso, mensaje, clientesLista);
        }

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.ProductosLista> Productos)> ObtenerProductos(Guid idRuta)
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var productosLista = new List<Models.Custom.ProductosLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerProductos(idRuta);
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        productosLista.Add(new Models.Custom.ProductosLista
                        {
                            Id = catalogo.Id,
                            Producto = catalogo.Producto
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                productosLista = new List<Models.Custom.ProductosLista>();
            }

            return (exitoso, mensaje, productosLista);
        }

    }
}
