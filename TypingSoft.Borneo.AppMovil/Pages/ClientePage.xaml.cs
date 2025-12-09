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
            if (seleccionado != null)
            {
                _clienteSeleccionado = seleccionado;
                // Mantener el texto del botón. Solo actualizar el label de preview.
                previewClienteLabel.Text = $"Seleccionado: {seleccionado.Cliente}";
                previewClienteLabel.IsVisible = true;
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
            ViewModel.ClientesASurtir.Clear();  
            ViewModel.ClientesASurtir.Add(clienteSeleccionado);

        }
        public async void OnRepartoClicked(object sender, EventArgs e)
        {
            if (ViewModel?.ClientesASurtir.Count == 0)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar al menos un cliente antes de continuar.", "OK");
                return;
            }


            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdCliente, _clienteSeleccionado.IdCliente.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdClienteAsociado, _clienteSeleccionado.IdClienteAsociado.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.Cliente, _clienteSeleccionado.Cliente ?? string.Empty);

            //var empleadoSeleccionado = Helpers.StaticSettings.ObtenerValor<string>("Empleado");
            //var condicionTexto = await ViewModel._localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(_clienteSeleccionado.IdClienteAsociado);

            //var nuevoTicket = new TypingSoft.Borneo.AppMovil.Local.TicketDetalleLocal
            //{
            //    Id = Guid.NewGuid(),
            //    IdCliente = _clienteSeleccionado.IdClienteAsociado,
            //    Cliente = _clienteSeleccionado.Cliente ?? string.Empty,
            //    Empleado = empleadoSeleccionado,
            //    Fecha = DateTime.Now,
            //    CondicionPago = condicionTexto
            //};
            //await ViewModel._localDb.InsertarTicketAsync(nuevoTicket);

            await ViewModel.Surtir(_clienteSeleccionado);

            // Reset tras añadir
            _clienteSeleccionado = null;
            btnSeleccionarCliente.Text = "Seleccionar cliente";
            previewClienteLabel.IsVisible = false;
            previewClienteLabel.Text = string.Empty;




            await Navigation.PushAsync(new RepartoPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_suspendRefresh) return;

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
            previewClienteLabel.IsVisible = false;
            previewClienteLabel.Text = string.Empty;
            ViewModel?.ClientesASurtir.Clear();
        }
    }
}