using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class RepartoVM : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;

        public RepartoVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;
            ListadoProductos = new ObservableCollection<Models.Custom.ProductosLista>();
            ListadoFormas = new ObservableCollection<Models.Custom.FormasLista>();
            ListadoCondiciones = new ObservableCollection<Models.Custom.CondicionesLista>();
            ListadoPrecios = new ObservableCollection<Models.Custom.PreciosLista>();
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;
        [ObservableProperty]
        ObservableCollection<Models.Custom.ProductosLista> listadoProductos;

        [ObservableProperty]
        ObservableCollection<Models.Custom.FormasLista> listadoFormas;

        [ObservableProperty]
        ObservableCollection<Models.Custom.CondicionesLista> listadoCondiciones;

        [ObservableProperty]
        ObservableCollection<Models.Custom.PreciosLista> listadoPrecios;



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

        public async Task CargarProductosDesdeLocal()
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
        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }


        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
    }
}
