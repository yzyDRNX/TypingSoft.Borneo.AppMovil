using TypingSoft.Borneo.AppMovil.BL;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace TypingSoft.Borneo.AppMovil.Services
{
    public class SincronizacionService
    {
        private readonly CatalogosBL _catalogos;
        private readonly LocalDatabaseService _localDb;

        public SincronizacionService(CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;
        }

        public async Task SincronizarCatalogosAsync()
        {
            // Empleados
            var resultadoEmp = await _catalogos.ObtenerEmpleados();
            if (resultadoEmp.Exitoso && resultadoEmp.Empleados != null && resultadoEmp.Empleados.Count > 0)
            {
                var empleadosLocales = resultadoEmp.Empleados.Select(e => new EmpleadoLocal
                {
                    Id = e.Id,
                    Empleado = e.Empleado
                }).ToList();
                try
                {
                    await _localDb.GuardarEmpleadosAsync(empleadosLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Empleados guardados: {empleadosLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando empleados: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][Empleados] No se actualiza tabla (Exitoso={resultadoEmp.Exitoso}, Count={resultadoEmp.Empleados?.Count ?? 0}).");
            }

            // Clientes
            if (Settings.IdRuta == Guid.Empty)
            {
                System.Diagnostics.Debug.WriteLine("[SYNC][Clientes] Settings.IdRuta vacío. Se omite sincronización de clientes.");
            }
            else
            {
                var resultadoCli = await _catalogos.ObtenerClientes(Settings.IdRuta);
                if (resultadoCli.Exitoso && resultadoCli.Clientes != null && resultadoCli.Clientes.Count > 0)
                {
                    var clientesLocales = resultadoCli.Clientes.Select(c => new ClienteLocal
                    {
                        IdCliente = c.IdCliente,
                        IdClienteAsociado = c.IdClienteAsociado,
                        Cliente = c.Cliente
                    }).ToList();
                    try
                    {
                        await _localDb.GuardarClientesAsync(clientesLocales);
                        System.Diagnostics.Debug.WriteLine($"[SQLite] Clientes guardados: {clientesLocales.Count}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando clientes: {ex.Message}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[SYNC][Clientes] No se sobreescribe tabla (Exitoso={resultadoCli.Exitoso}, Count={resultadoCli.Clientes?.Count ?? 0}). Se conservan clientes locales.");
                }
            }

            // Productos
            var (exitosoProd, mensajeProd, productos) = await _catalogos.ObtenerProductos();
            if (exitosoProd && productos != null && productos.Count > 0)
            {
                var productosLocales = productos.Select(p => new ProductoLocal
                {
                    Id = p.Id,
                    Producto = p.Producto
                }).ToList();
                try
                {
                    await _localDb.GuardarProductosAsync(productosLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Productos guardados: {productosLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando productos: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][Productos] No se actualiza tabla (Exitoso={exitosoProd}, Count={productos?.Count ?? 0}).");
            }

            // Formas
            var (exitosoForm, mensajeForm, formas) = await _catalogos.ObtenerFormas();
            if (exitosoForm && formas != null && formas.Count > 0)
            {
                var formasLocales = formas.Select(f => new FormaLocal
                {
                    IdForma = f.IdForma,
                    Forma = f.Forma
                }).ToList();
                try
                {
                    await _localDb.GuardarFormasAsync(formasLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Formas guardadas: {formasLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando formas: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][Formas] No se actualiza tabla (Exitoso={exitosoForm}, Count={formas?.Count ?? 0}).");
            }

            // Condiciones
            var (exitosoCond, mensajeCond, condiciones) = await _catalogos.ObtenerCondiciones();
            if (exitosoCond && condiciones != null && condiciones.Count > 0)
            {
                var condicionesLocales = condiciones.Select(c => new CondicionLocal
                {
                    IdCondicion = c.IdCondicion,
                    Condicion = c.Condicion
                }).ToList();
                try
                {
                    await _localDb.GuardarCondicionesAsync(condicionesLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Condiciones guardadas: {condicionesLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando condiciones: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][Condiciones] No se actualiza tabla (Exitoso={exitosoCond}, Count={condiciones?.Count ?? 0}).");
            }

            // Precios Generales
            var (exitosoPrec, mensajePrec, precios) = await _catalogos.ObtenerPreciosGenerales();
            if (exitosoPrec && precios != null && precios.Count > 0)
            {
                var preciosLocales = precios.Select(p => new PreciosGeneralesLocal
                {
                    IdProducto = p.IdProducto,
                    Producto = p.Producto,
                    Precio = p.Precio.ToString()
                }).ToList();
                try
                {
                    await _localDb.GuardarPreciosGeneralesAsync(preciosLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Precios generales guardados: {preciosLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando precios: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][PreciosGenerales] No se actualiza tabla (Exitoso={exitosoPrec}, Count={precios?.Count ?? 0}).");
            }

            // Precios Preferenciales
            var (exitosoPrecPref, mensajePrecPref, preciosPref) = await _catalogos.ObtenerPreciosPreferenciales();
            if (exitosoPrecPref && preciosPref != null && preciosPref.Count > 0)
            {
                var preciosLocales = preciosPref.Select(p => new PreciosPreferencialesLocal
                {
                    IdProducto = p.IdProducto,
                    Producto = p.Producto,
                    Precio = p.Precio.ToString(),
                    IdClienteAsociado = p.IdClienteAsociado
                }).ToList();
                try
                {
                    await _localDb.GuardarPreciosPreferencialesAsync(preciosLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Precios preferenciales guardados: {preciosLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando precios preferenciales: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][PreciosPref] No se actualiza tabla (Exitoso={exitosoPrecPref}, Count={preciosPref?.Count ?? 0}).");
            }

            // Facturación
            var (exitosoFact, mensajeFact, facturacion) = await _catalogos.ObtenerFacturacion();
            if (exitosoFact && facturacion != null && facturacion.Count > 0)
            {
                var facturacionLocales = facturacion.Select(f => new FacturacionLocal
                {
                    Id = f.Id,
                    IdAsociado = f.IdAsociado,
                    RazonSocial = f.RazonSocial,
                    Calle = f.Calle,
                    NumeroExterior = f.NumeroExterior,
                    NumeroInterior = f.NumeroInterior,
                    Colonia = f.Colonia,
                    CP = f.CP,
                    Municipio = f.Municipio,
                    Estado = f.Estado,
                    IdFormaPago = f.IdFormapago,
                    IdMetodoPago = f.IdMetodoPago,
                    IdUsoCFDI = f.IdUsoCFDI
                }).ToList();
                try
                {
                    await _localDb.GuardarFacturacionAsync(facturacionLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Facturación guardada: {facturacionLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando facturación: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][Facturación] No se actualiza tabla (Exitoso={exitosoFact}, Count={facturacion?.Count ?? 0}).");
            }

            // Clientes Aplicaciones
            var (exitosoCliApp, mensajeCliApp, clientesAplicaciones) = await _catalogos.ObtenerClientesAplicaciones();
            if (exitosoCliApp && clientesAplicaciones != null && clientesAplicaciones.Count > 0)
            {
                var clientesAplicacionesLocales = clientesAplicaciones.Select(c => new ClientesAplicacionesLocal
                {
                    Id = c.Id,
                    IdClienteAsociado = c.IdClienteAsociado,
                    AplicaAPP = c.AplicaAPP,
                    AplicaMuestraPrecio = c.AplicaMuestraPrecio,
                    AplicaComodato = c.AplicaComodato,
                    AplicaDescuentos = c.AplicaDescuentos,
                    AplicaFacturacion = c.AplicaFacturacion,
                    AplicaCobranza = c.AplicaCobranza,
                    AplicaVales = c.AplicaVales,
                    AplicaMultiRuta = c.AplicaMultiRuta,
                }).ToList();
                try
                {
                    await _localDb.GuardarClientesAplicacionesAsync(clientesAplicacionesLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Clientes aplicaciones guardados: {clientesAplicacionesLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando clientes aplicaciones: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][ClientesAplicaciones] No se actualiza tabla (Exitoso={exitosoCliApp}, Count={clientesAplicaciones?.Count ?? 0}).");
            }

            // Condiciones de Pago por Cliente
            var (exitosoCondCli, mensajeCondCli, condicionesPago) = await _catalogos.ObtenerClientesCondiciones();
            if (exitosoCondCli && condicionesPago != null && condicionesPago.Count > 0)
            {
                var condicionesPagoLocales = condicionesPago.Select(c => new CondicionPagoLocal
                {
                    Id = c.Id,
                    IdClienteAsociado = c.IdClienteAsociado,
                    IdCondicionPago = c.IdCondicionPago
                }).ToList();

                try
                {
                    await _localDb.GuardarCondicionesPagoAsync(condicionesPagoLocales);
                    System.Diagnostics.Debug.WriteLine($"[SQLite] Condiciones de pago por cliente guardadas: {condicionesPagoLocales.Count}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando condiciones de pago por cliente: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[SYNC][CondicionesPagoCliente] No se actualiza tabla (Exitoso={exitosoCondCli}, Count={condicionesPago?.Count ?? 0}).");
            }
        }
    }
}
