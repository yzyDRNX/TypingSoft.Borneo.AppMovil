using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class CatalogosVM : Helpers.VMBase
    {
        #region Campos y constructor
        private readonly BL.CatalogosBL _catalogos;
        private readonly LocalDatabaseService _localDb;

        public CatalogosVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>();
            ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>();
            ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>();
        }
        #endregion

        #region Propiedades observables
        [ObservableProperty]
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;

        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;

        [ObservableProperty]
        ObservableCollection<Models.Custom.ProductosLista> listadoProductos;
        #endregion

        #region Métodos


        public async Task ObtenerEmpleadosAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Empleados";
                Procesando = true;

                // Llamada a la API
                var (exitoso, mensaje, listaEmpleados) = await _catalogos.ObtenerEmpleados();

                if (exitoso)
                {
                    // Actualiza la UI
                    ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(listaEmpleados);

                    // Convierte y guarda en SQLite
                    var empleadosLocales = listaEmpleados
                        .Select(e => new Local.EmpleadoLocal { Id = e.Id, Nombre = e.Empleado })
                        .ToList();

                    await _localDb.GuardarEmpleadosAsync(empleadosLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener empleados.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }


        public async Task ObtenerClientesAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Clientes";
                Procesando = true;

                Guid idRuta = Helpers.Settings.IdRuta;
                var (exitoso, mensaje, listaClientes) = await _catalogos.ObtenerClientes(idRuta);

                if (exitoso)
                {
                    ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>(listaClientes);

                    // Convierte y guarda en SQLite
                    var clientesLocales = listaClientes
                        .Select(c => new Local.ClienteLocal { IdCliente = c.IdCliente, IdClienteAsociado = c.IdClienteAsociado, Cliente = c.Cliente })
                        .ToList();

                    await _localDb.GuardarClientesAsync(clientesLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener clientes.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }

        public async Task ObtenerProductosAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Productos";
                Procesando = true;

                Guid idRuta = Helpers.Settings.IdRuta;
                var (exitoso, mensaje, listaProductos) = await _catalogos.ObtenerProductos(idRuta);

                if (exitoso)
                {
                    ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>(listaProductos);

                    // Convierte y guarda en SQLite
                    var productosLocales = listaProductos
                        .Select(p => new Local.ProductoLocal { Id = p.Id, Producto = p.Producto })
                        .ToList();

                    await _localDb.GuardarProductosAsync(productosLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener productos.");
                }
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Excepción", ex.Message);
            }
            finally
            {
                Procesando = false;
            }
        }

        #endregion

        #region Alerta 
        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}