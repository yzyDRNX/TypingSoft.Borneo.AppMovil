using Microsoft.Maui.Controls;
using System;
using System.Linq;
using TypingSoft.Borneo.AppMovil.Pages.Modals;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class ClientePage : ContentPage
    {
        VModels.ClientePageViewModel ViewModel;
        private Models.Custom.ClientesLista? _clienteSeleccionado;

        // Evita recargas/limpiezas cuando vienes de un modal y carga solo 1 vez
        private bool _suspendRefresh;
        private bool _initialized;

        public ClientePage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.ClientePageViewModel>();
            if (ViewModel != null)
                this.BindingContext = ViewModel;

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Sin resetear selección ni texto del botón aquí
        }

        private async void OnSeleccionarClienteClicked(object sender, EventArgs e)
        {
            if (ViewModel?.ListadoClientes == null || ViewModel.ListadoClientes.Count == 0)
            {
                await DisplayAlert("Aviso", "No hay clientes cargados.", "OK");
                return;
            }

            _suspendRefresh = true;
            var modal = new SelectClienteModal(ViewModel.ListadoClientes);
            var seleccionado = await modal.ShowAsync(Navigation);
            // Mantén suspendido hasta después de procesar selección
            if (seleccionado != null)
            {
                _clienteSeleccionado = seleccionado;
                btnSeleccionarCliente.Text = seleccionado.Cliente ?? "Cliente seleccionado";
            }
            _suspendRefresh = false;
        }

        private async void OnAñadirClienteClicked(object sender, EventArgs e)
        {
            var clienteSeleccionado = _clienteSeleccionado;
            if (clienteSeleccionado == null)
            {
                await DisplayAlert("Aviso", "Por favor seleccione un cliente.", "OK");
                return;
            }

            if (ViewModel.ClientesASurtir.Count > 0)
            {
                await DisplayAlert("Aviso", "Solo puede añadir un cliente a la vez.", "OK");
                return;
            }

            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdCliente, clienteSeleccionado.IdCliente.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdClienteAsociado, clienteSeleccionado.IdClienteAsociado.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.Cliente, clienteSeleccionado.Cliente ?? string.Empty);

            var empleadoSeleccionado = Helpers.StaticSettings.ObtenerValor<string>("Empleado");

            var nuevoTicket = new TypingSoft.Borneo.AppMovil.Local.TicketDetalleLocal
            {
                Id = Guid.NewGuid(),
                IdCliente = clienteSeleccionado.IdClienteAsociado,
                Cliente = clienteSeleccionado.Cliente ?? string.Empty,
                Empleado = empleadoSeleccionado,
                Fecha = DateTime.Now
            };
            await ViewModel._localDb.InsertarTicketAsync(nuevoTicket);

            await ViewModel.Surtir(clienteSeleccionado);

            // Reset tras añadir
            _clienteSeleccionado = null;
            btnSeleccionarCliente.Text = "Seleccionar cliente";
        }

        public async void OnRepartoClicked(object sender, EventArgs e)
        {
            if (ViewModel?.ClientesASurtir.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar al menos un cliente antes de continuar.", "OK");
                return;
            }
            await Navigation.PushAsync(new RepartoPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // No limpies ni recargues cuando vienes de modal
            if (_suspendRefresh) return;

            // Cargar datos solo la primera vez
            if (!_initialized && ViewModel != null)
            {
                await ViewModel.CargarClientesDesdeLocal();
                _initialized = true;
            }
        }

        public void LimpiarCamposYListas()
        {
            _clienteSeleccionado = null;
            btnSeleccionarCliente.Text = "Seleccionar cliente";
            ViewModel?.ClientesASurtir.Clear();
        }
    }
}