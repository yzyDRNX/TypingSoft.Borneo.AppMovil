using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class ClientePageViewModel : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;


        public ClientePageViewModel(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>();
            // Inicializar la propiedad DescripcionRuta
            DescripcionRuta = "Cargando descripción...";
            CargarDescripcionRuta();
        }
        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;
        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> clientesASurtir = new();

        private string _descripcionRuta;
        public string DescripcionRuta
        {
            get => _descripcionRuta;
            set
            {
                _descripcionRuta = value;
                OnPropertyChanged(nameof(DescripcionRuta));
            }
        }

        public ClientePageViewModel() // <-- Corregido aquí
        {
            CargarDescripcionRuta();
        }

        private async void CargarDescripcionRuta()
        {
            var db = new LocalDatabaseService();
            DescripcionRuta = await db.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
        public async Task Surtir(Models.Custom.ClientesLista cliente)
        {
            if (cliente != null && !ClientesASurtir.Contains(cliente))
            {
                clientesASurtir.Add(cliente);

                // Guarda el IdClienteAsociado del cliente seleccionado
                Helpers.Settings.IdClienteAsociado = cliente.IdClienteAsociado;
            }

            // Mostrar la lista actual de clientes a surtir
            var nombres = string.Join("\n", ClientesASurtir.Select(c => c.Cliente));
            await Application.Current.MainPage.DisplayAlert("Clientes a Surtir", nombres, "OK");
        }
        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
    }
}
