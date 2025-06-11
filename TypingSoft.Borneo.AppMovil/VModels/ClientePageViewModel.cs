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
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();

        }
        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;
        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> clientesASurtir = new();

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;

       

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

       

        public async Task CargarClientesDesdeLocal()
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
