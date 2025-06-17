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
            if (resultadoEmp.Exitoso && resultadoEmp.Empleados != null)
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

            // Clientes
            var resultadoCli = await _catalogos.ObtenerClientes(Settings.IdRuta);
            if (resultadoCli.Exitoso && resultadoCli.Clientes != null)
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

            // Productos
            var (exitosoProd, mensajeProd, productos) = await _catalogos.ObtenerProductos();
            if (exitosoProd && productos != null)
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

            // Formas
            var (exitosoForm, mensajeForm, formas) = await _catalogos.ObtenerFormas();
            if (exitosoForm && formas != null)
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

            // Condiciones
            var (exitosoCond, mensajeCond, condiciones) = await _catalogos.ObtenerCondiciones();
            if (exitosoCond && condiciones != null)
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

            // Precios Generales
            var (exitosoPrec, mensajePrec, precios) = await _catalogos.ObtenerPreciosGenerales();
            if (exitosoPrec && precios != null)
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



            var (exitosoPrecPref, mensajePrecPref, preciosPref) = await _catalogos.ObtenerPreciosPreferenciales();
            if (exitosoPrecPref && preciosPref != null)
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
                    System.Diagnostics.Debug.WriteLine($"[SQLite][Error] Guardando precios: {ex.Message}");
                }
            }
        }


    }
}
