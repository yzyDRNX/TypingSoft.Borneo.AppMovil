using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class UtileriasPageViewModel : Helpers.VMBase
    {
        private string _fechaActual;
        public string FechaActual
        {
            get => _fechaActual;
            set
            {
                _fechaActual = value;
                OnPropertyChanged();
            }
        }

        private VentaGeneralResponse _ventaActual;
        public VentaGeneralResponse VentaActual
        {
            get => _ventaActual;
            set
            {
                _ventaActual = value;
                OnPropertyChanged();
            }
        }

        private string _descripcionRuta;
        public string DescripcionRuta
        {
            get => _descripcionRuta;
            set
            {
                _descripcionRuta = value;
                OnPropertyChanged();
            }
        }

        public UtileriasPageViewModel()
        {
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
        }

        private async void CargarDescripcionRuta()
        {
            var db = new LocalDatabaseService();
            DescripcionRuta = await db.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        // Comando de impresión
        [RelayCommand]
        public async Task ImprimirAsync()
        {
            if (VentaActual == null)
                return;

            string ticket = TicketFormatter.FormatearTicket(VentaActual);

            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
            if (printer != null)
            {
                await printer.PrintTextAsync(ticket);
            }
            // Si quieres manejar errores aquí, puedes loguearlos o mostrar un Toast si tienes acceso
        }
    }
}
