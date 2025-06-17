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
        public readonly LocalDatabaseService _localDb;


        public CatalogosVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>();
            ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>();
            ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>();
            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>();
            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>();
            ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>();
            ListadoPreciosPreferenciales = new ObservableCollection<Models.Custom.PreciosPreferencialesLista>();

            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }

        #endregion

        #region Propiedades observables
        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;

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
        ObservableCollection<Models.Custom.PreciosGeneralesLista> listadoPreciosGenerales;
        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosPreferencialesLista> listadoPreciosPreferenciales;
        #endregion

        #region Métodos

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }


        [RelayCommand]
        public async Task ObtenerEmpleadosAsync()
        {
            MensajeProcesando = "Cargando Empleados...";
            Procesando = true;

            try
            {
                // Llamada a la API
                var (exitoso, mensaje, listaEmpleados) = await _catalogos.ObtenerEmpleados();

                if (exitoso && listaEmpleados != null && listaEmpleados.Any())
                {
                    // Actualiza la UI
                    ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(listaEmpleados);

                    // Convierte y guarda en SQLite
                    var empleadosLocales = listaEmpleados
                        .Select(e => new Local.EmpleadoLocal
                        {
                            Id = e.Id,
                            Empleado = e.Empleado
                            // Agrega más campos si tu modelo local tiene más
                        })
                        .ToList();

                    await _localDb.GuardarEmpleadosAsync(empleadosLocales);

                    MensajeProcesando = "Empleados actualizados desde servidor.";
                }
                else
                {
                    await CargarEmpleadosDesdeLocal("Fallo al obtener empleados del servidor.");
                }
            }
            catch (Exception ex)
            {
                await CargarEmpleadosDesdeLocal($"Error inesperado: {ex.Message}");
            }
            finally
            {
                Procesando = false;
            }
        }

        private async Task CargarEmpleadosDesdeLocal(string mensajeError)
        {
            var empleadosLocales = await _localDb.ObtenerEmpleadosAsync();

            if (empleadosLocales != null && empleadosLocales.Any())
            {
                ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(
                    empleadosLocales.Select(e => new Models.Custom.EmpleadosLista
                    {
                        Id = e.Id,
                        Empleado = e.Empleado
                        // Ajusta aquí si usas más campos como Apellidos, etc.
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando empleados locales.");
            }
            else
            {
                await MostrarAlertaAsync("Error", mensajeError + "\nNo hay datos locales disponibles.");
            }
        }



        [RelayCommand]
        public async Task ObtenerClientesAsync()
        {
            MensajeProcesando = "Cargando Clientes...";
            Procesando = true;

            try
            {
                // Revisar idRuta válido en Settings
                Guid idRuta = Helpers.Settings.IdRuta;
                if (idRuta == Guid.Empty)
                {
                    // Si no hay ruta en Settings, paso directamente al modo offline
                    await CargarClientesDesdeLocal();
                    return;
                }

                // llamar a la API
                var (exitoso, mensaje, listaClientes) = await _catalogos.ObtenerClientes(idRuta);

                if (exitoso && listaClientes != null)
                {
                    ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>(listaClientes);

                    // Guardar en SQLite, poblando la entidad local
                    var clientesLocales = listaClientes
                        .Select(c => new Local.ClienteLocal
                        {
                            IdCliente = c.IdCliente,
                            IdClienteAsociado = c.IdClienteAsociado,
                            Cliente = c.Cliente
                        })
                        .ToList();

                    await _localDb.GuardarClientesAsync(clientesLocales);

                    MensajeProcesando = "Clientes actualizados desde servidor.";
                }
                else
                {
                    // Si la API no devolvió datos (exitoso == false o lista == null), voy a offline
                    await CargarClientesDesdeLocal();
                }
            }
            catch (Exception ex)
            {
                // Si ocurrió cualquier excepción voy a offline
                await CargarClientesDesdeLocal();
            }
            finally
            {
                Procesando = false;
            }
        }

        private async Task CargarClientesDesdeLocal()
        {
            // Obtengo el IdRuta que guardé previamente en BD local
            Guid? idRutaLocal = await _localDb.ObtenerIdRutaAsync();
            if (!idRutaLocal.HasValue || idRutaLocal.Value == Guid.Empty)
            {
                await MostrarAlertaAsync("Advertencia", "No hay ruta local válida.");
                return;
            }

            // Recupero sólo los clientes de esa ruta
            var clientesLocales = await _localDb.ObtenerClientesAsync(idRutaLocal.Value);
            if (clientesLocales != null && clientesLocales.Any())
            {
                ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>(
                    clientesLocales.Select(c => new Models.Custom.ClientesLista
                    {
                        IdCliente = c.IdCliente,
                        IdClienteAsociado = c.IdClienteAsociado,
                        Cliente = c.Cliente
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando clientes locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay datos locales.");
            }
        }

        [RelayCommand]
        public async Task ObtenerProductosAsync()
        {
            MensajeProcesando = "Cargando Productos...";
            Procesando = true;

            try
            {
                // Llamada a la API
                var (exitoso, mensaje, listaProductos) = await _catalogos.ObtenerProductos();

                if (exitoso && listaProductos != null)
                {
                    ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>(listaProductos);

                    // Convertir a entidad local y guardar en SQLite
                    var productosLocales = listaProductos
                        .Select(p => new Local.ProductoLocal
                        {
                            Id = p.Id,
                            Producto = p.Producto
                        })
                        .ToList();

                    await _localDb.GuardarProductosAsync(productosLocales);

                    MensajeProcesando = "Productos actualizados desde servidor.";
                }
                else
                {
                    // Si falla la API o devuelve vacío, carga local
                    await CargarProductosDesdeLocal();
                }
            }
            catch (Exception)
            {
                // En caso de error, cargar productos desde almacenamiento local
                await CargarProductosDesdeLocal();
            }
            finally
            {
                Procesando = false;
            }
        }

        private async Task CargarProductosDesdeLocal()
        {
            var productosLocales = await _localDb.ObtenerProductosAsync();

            if (productosLocales != null && productosLocales.Any())
            {
                ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>(
                    productosLocales.Select(p => new Models.Custom.ProductosLista
                    {
                        Id = p.Id,
                        Producto = p.Producto
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando productos locales.");
            }
            else
            {
                await MostrarAlertaAsync("Advertencia", "No hay productos locales disponibles.");
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
                var (exitoso, mensaje, listaPrecios) = await _catalogos.ObtenerPreciosGenerales();

                if (exitoso)
                {
                    ListadoPreciosGenerales = new ObservableCollection<Models.Custom.PreciosGeneralesLista>(listaPrecios);

                    // Convierte y guarda en SQLite
                    var preciosLocales = listaPrecios
                        .Select(p => new Local.PreciosGeneralesLocal { IdProducto = p.IdProducto, Producto = p.Producto, Precio = p.Precio.ToString() })
                        .ToList();

                    await _localDb.GuardarPreciosGeneralesAsync(preciosLocales);
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
        public async Task Surtir(Models.Custom.ClientesLista cliente)
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
        [RelayCommand]
        public async Task GuardarClientesTemporalAsync()
        {
            if (ClientesASurtir.Count == 0)
            {
                await MostrarAlertaAsync("Aviso", "No hay clientes seleccionados para guardar.");
                return;
            }
            try
            {
                // Guardar los clientes seleccionados en la base de datos local
                var clientesLocales = ClientesASurtir.Select(c => new Local.ClienteLocal
                {
                    IdCliente = c.IdCliente,
                    Cliente = c.Cliente
                }).ToList();
                await _localDb.GuardarClientesAsync(clientesLocales);
                await MostrarAlertaAsync("Éxito", "Clientes guardados temporalmente.");
            }
            catch (Exception ex)
            {
                await MostrarAlertaAsync("Error", $"No se pudieron guardar los clientes: {ex.Message}");
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