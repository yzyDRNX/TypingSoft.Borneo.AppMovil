using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.VModels;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class ClientePage : ContentPage
    {
        private CatalogosVM ViewModel => BindingContext as CatalogosVM;
        

        public ClientePage()
        {
            InitializeComponent();
            SetupViewModel();
        }

        private void SetupViewModel()
        {
            if (App.ServiceProvider != null)
            {
                var viewModel = App.ServiceProvider.GetService<CatalogosVM>();
                this.BindingContext = viewModel ?? CreateFallbackViewModel();
            }
            else
            {
                this.BindingContext = CreateFallbackViewModel();
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
            if (ViewModel == null) return;

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

            await ViewModel.Surtir(clienteSeleccionado);
            await ViewModel._localDb.GuardarClienteTemporalAsync(clienteSeleccionado.Cliente); // ← Guardar en base local

            clientesPicker.SelectedItem = null; // Resetear selección
        }


        private async void OnRepartoClicked(object sender, EventArgs e)
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

            if (ViewModel != null)
            {
                await ViewModel.ObtenerClientesAsync();
            }

            clientesPicker.SelectedItem = null;
        }
    }
}