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
            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>();
            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>();
            ListadoPrecios = new ObservableCollection<Models.Custom.PreciosLista>();

        }
        #endregion

        #region Propiedades observables
        [ObservableProperty]
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;

        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;

        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> clientesASurtir = new();


        [ObservableProperty]
        ObservableCollection<Models.Custom.ProductosLista> listadoProductos;

        [ObservableProperty]
        ObservableCollection<Models.Custom.FormasLista> listadoFormas;

        [ObservableProperty]
        ObservableCollection<Models.Custom.CondicionesLista> listadoCondiciones;

        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosLista> listadoPrecios;
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

                var (exitoso, mensaje, listaProductos) = await _catalogos.ObtenerProductos();

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

        public async Task ObtenerFormasAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Formas";
                Procesando = true;

                var (exitoso, mensaje, listaFormas) = await _catalogos.ObtenerFormas();

                if (exitoso)
                {
                    ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>(listaFormas);

                    // Convierte y guarda en SQLite
                    var formasLocales = listaFormas
                        .Select(f => new Local.FormaLocal { IdForma = f.IdForma, Forma = f.Forma })
                        .ToList();

                    await _localDb.GuardarFormasAsync(formasLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
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

        public async Task ObtenerCondicionesAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Condiciones";
                Procesando = true;

                var (exitoso, mensaje, listaCondiciones) = await _catalogos.ObtenerCondiciones();

                if (exitoso)
                {
                    ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>(listaCondiciones);

                    // Convierte y guarda en SQLite
                    var condicionesLocales = listaCondiciones
                        .Select(c => new Local.CondicionLocal { IdCondicion = c.IdCondicion, Condicion = c.Condicion })
                        .ToList();

                    await _localDb.GuardarCondicionesAsync(condicionesLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener formas.");
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

        public async Task ObtenerPreciosAsync()
        {
            try
            {
                MensajeProcesando = "Cargando Precios";
                Procesando = true;

                Guid IdClienteAsociado = Helpers.Settings.IdClienteAsociado;
                var (exitoso, mensaje, listaPrecios) = await _catalogos.ObtenerPrecios(IdClienteAsociado);

                if (exitoso)
                {
                    ListadoPrecios = new ObservableCollection<Models.Custom.PreciosLista>(listaPrecios);

                    // Convierte y guarda en SQLite
                    var preciosLocales = listaPrecios
                        .Select(p => new Local.PrecioLocal { IdProducto = p.IdProducto, Producto = p.Producto, Precio = p.Precio.ToString() })
                        .ToList();

                    await _localDb.GuardarPreciosAsync(preciosLocales);
                }
                else
                {
                    await MostrarAlertaAsync("Error", mensaje ?? "Fallo al obtener Precios.");
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

        [RelayCommand]
        async Task Surtir(Models.Custom.ClientesLista cliente)
        {
            if (cliente != null && !ClientesASurtir.Contains(cliente))
            {
                ClientesASurtir.Add(cliente);

                // Guarda el IdClienteAsociado del cliente seleccionado
                Helpers.Settings.IdClienteAsociado = cliente.IdClienteAsociado;
            }

            // Mostrar la lista actual de clientes a surtir
            var nombres = string.Join("\n", ClientesASurtir.Select(c => c.Cliente));
            await Application.Current.MainPage.DisplayAlert("Clientes a Surtir", nombres, "OK");
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