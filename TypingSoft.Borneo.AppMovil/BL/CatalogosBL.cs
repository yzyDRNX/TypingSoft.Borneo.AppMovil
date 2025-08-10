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
                System.Diagnostics.Debug.WriteLine($"StatusCode: {peticion.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {peticion.Respuesta?.Mensaje}");
                System.Diagnostics.Debug.WriteLine($"Data: {peticion.Respuesta?.Data}");
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

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.ProductosLista> Productos)> ObtenerProductos()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var productosLista = new List<Models.Custom.ProductosLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerProductos();
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

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.FormasLista> Formas)> ObtenerFormas()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var formasLista = new List<Models.Custom.FormasLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerFormas();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        formasLista.Add(new Models.Custom.FormasLista
                        {
                            IdForma = catalogo.IdForma,
                            Forma = catalogo.Forma
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                formasLista = new List<Models.Custom.FormasLista>();
            }

            return (exitoso, mensaje, formasLista);
        }

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.CondicionesLista> Condiciones)> ObtenerCondiciones()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var condicionesLista = new List<Models.Custom.CondicionesLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerCondiciones();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        condicionesLista.Add(new Models.Custom.CondicionesLista
                        {
                            IdCondicion = catalogo.IdCondicion,
                            Condicion = catalogo.Condicion
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                condicionesLista = new List<Models.Custom.CondicionesLista>();
            }

            return (exitoso, mensaje, condicionesLista);
        }

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.PreciosGeneralesLista> Precios)> ObtenerPreciosGenerales()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var preciosLista = new List<Models.Custom.PreciosGeneralesLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerPreciosGenerales();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        preciosLista.Add(new Models.Custom.PreciosGeneralesLista
                        {
                            IdProducto = catalogo.IdProducto,
                            Producto = catalogo.Producto,
                            Precio = catalogo.Precio
                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                preciosLista = new List<Models.Custom.PreciosGeneralesLista>();
            }

            return (exitoso, mensaje, preciosLista);
        }

        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.PreciosPreferencialesLista> PreciosPref)> ObtenerPreciosPreferenciales()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var preciosLista = new List<Models.Custom.PreciosPreferencialesLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerPreciosPreferenciales();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        preciosLista.Add(new Models.Custom.PreciosPreferencialesLista
                        {
                            IdProducto = catalogo.IdProducto,
                            Producto = catalogo.Producto,
                            Precio = catalogo.Precio,
                            IdClienteAsociado = catalogo.IdClienteAsociado

                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                preciosLista = new List<Models.Custom.PreciosPreferencialesLista>();
            }

            return (exitoso, mensaje, preciosLista);
        }
        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.FacturacionLista> PreciosPref)> ObtenerFacturacion()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var preciosLista = new List<Models.Custom.FacturacionLista>();

            try
            {
                var peticion = await this.CatalogosService.ObtenerFacturacion();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        preciosLista.Add(new Models.Custom.FacturacionLista
                        {
                            Id = catalogo.Id,
                            IdAsociado = catalogo.IdAsociado,
                            RazonSocial = catalogo.RazonSocial,
                            Calle = catalogo.Calle,
                            NumeroExterior = catalogo.NumeroExterior,
                            NumeroInterior = catalogo.NumeroInterior,
                            Colonia = catalogo.Colonia,
                            CP = catalogo.CP,
                            Municipio = catalogo.Municipio,
                            Estado = catalogo.Estado,
                            IdFormapago = catalogo.IdFormapago,
                            IdMetodoPago = catalogo.IdMetodoPago,
                            IdUsoCFDI = catalogo.IdUsoCFDI

                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                preciosLista = new List<Models.Custom.FacturacionLista>();
            }

            return (exitoso, mensaje, preciosLista);
        }
        public async Task<(bool Exitoso, string Mensaje, List<Models.Custom.ClientesAplicacionesLista> ClientesAplicaciones)> ObtenerClientesAplicaciones()
        {
            var exitoso = false;
            var mensaje = "Ocurrió un error en la petición";
            var clientesAplicacionesLista = new List<Models.Custom.ClientesAplicacionesLista>();
            try
            {
                var peticion = await this.CatalogosService.ObtenerClientesAplicaciones();
                exitoso = peticion.StatusCode == System.Net.HttpStatusCode.OK;
                if (exitoso)
                {
                    foreach (var catalogo in peticion.Respuesta.Data)
                    {
                        clientesAplicacionesLista.Add(new Models.Custom.ClientesAplicacionesLista
                        {
                            Id = catalogo.Id,
                            IdClienteAsociado = catalogo.IdClienteAsociado,
                            AplicaAPP = catalogo.AplicaAPP,
                            AplicaMuestraPrecio = catalogo.AplicaMuestraPrecio,
                            AplicaComodato = catalogo.AplicaComodato,
                            AplicaDescuentos = catalogo.AplicaDescuentos,
                            AplicaFacturacion = catalogo.AplicaFacturacion,
                            AplicaCobranza = catalogo.AplicaCobranza,
                            AplicaVales = catalogo.AplicaVales,
                            AplicaMultiRuta = catalogo.AplicaMultiRuta

                        });
                    }
                }
            }
            catch (Exception)
            {
                mensaje = "Ocurrió un error en la petición";
                clientesAplicacionesLista = new List<Models.Custom.ClientesAplicacionesLista>();
            }
            return (exitoso, mensaje, clientesAplicacionesLista);

        }
    }
}
