using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq; // <-- Asegúrate de tener esto
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.VModels;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class ClientePage : ContentPage
    {
        VModels.ClientePageViewModel ViewModel;


        public ClientePage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetService<VModels.ClientePageViewModel>();
            if (ViewModel != null)
            {
                this.BindingContext = ViewModel;

            }

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        }
        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                
            }
        }

        private CatalogosVM CreateFallbackViewModel()
        {
            var catalogosService = new Services.CatalogosService();
            var catalogosBL = new BL.CatalogosBL(catalogosService);
            var localDb = new Services.LocalDatabaseService();
            return new CatalogosVM(catalogosBL, localDb);
        }

        private async void OnAñadirClienteClicked(object sender, EventArgs e)
        {
            var clienteSeleccionado = clientesPicker.SelectedItem as Models.Custom.ClientesLista;
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

            // Guarda el IdClienteAsociado y el IdCliente del cliente seleccionado
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdCliente, clienteSeleccionado.IdCliente.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.IdClienteAsociado, clienteSeleccionado.IdClienteAsociado.ToString());
            Helpers.StaticSettings.FijarConfiguracion(Helpers.StaticSettings.Cliente, clienteSeleccionado.Cliente ?? string.Empty);

            // --- CREA UN NUEVO TICKET CABECERA ---
            var empleadoSeleccionado = Helpers.StaticSettings.ObtenerValor<string>("Empleado"); // Recupera el empleado guardado

            var nuevoTicket = new TypingSoft.Borneo.AppMovil.Local.TicketDetalleLocal
            {
                Id = Guid.NewGuid(),
                IdCliente = clienteSeleccionado.IdClienteAsociado,
                Cliente = clienteSeleccionado.Cliente ?? string.Empty,
                Empleado = empleadoSeleccionado, // Aquí sí se asigna correctamente
                Fecha = DateTime.Now
            };
            await ViewModel._localDb.InsertarTicketAsync(nuevoTicket);

            await ViewModel.Surtir(clienteSeleccionado);

            clientesPicker.SelectedItem = null;
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

            // Limpia la lista de clientes a surtir al entrar a la página
            LimpiarCamposYListas();

            // Cargar clientes locales al aparecer la página
            if (ViewModel != null)
                await ViewModel.CargarClientesDesdeLocal();

            // Animación de fade-in para el frame principal
            await Task.WhenAll(
                clientesPicker.FadeTo(1, 500, Easing.CubicIn),
                clientesPicker.ScaleTo(1, 500, Easing.CubicOut)
            );
        }


        public void LimpiarCamposYListas()
        {
            clientesPicker.SelectedItem = null;
            ViewModel?.ClientesASurtir.Clear();
            // Otros campos si es necesario
        }
    }
}