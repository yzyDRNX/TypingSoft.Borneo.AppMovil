using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TypingSoft.Borneo.AppMovil.Services;
using TypingSoft.Borneo.AppMovil.Local;
using System.Linq;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class UtileriasPageViewModel : Helpers.VMBase
    {
        private readonly LocalDatabaseService _localDb;
        private int _numeroImpresiones = 0;
        private TicketLocal _ultimoTicket;

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
            _localDb = new LocalDatabaseService();
            FechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            CargarDescripcionRuta();
            _ = CargarUltimoTicketAsync();
        }

        private async void CargarDescripcionRuta()
        {
            DescripcionRuta = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }

        private async Task CargarUltimoTicketAsync()
        {
            var tickets = await _localDb.ObtenerTicketsAsync();
            _ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();
        }

        private async Task ImprimirTicketAsync(TicketLocal ticket)
        {
            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
            if (printer != null)
            {
                // Imprime ORIGINAL
                string ticketOriginal = TicketFormatter.FormatearTicketLocal(ticket, 1);
                await printer.PrintTextAsync(ticketOriginal);

                // Imprime REIMPRESION
                string ticketReimpresion = TicketFormatter.FormatearTicketLocal(ticket, 2);
                await printer.PrintTextAsync(ticketReimpresion);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se encontró el servicio de impresión.", "OK");
            }
        }

        // Comando de impresión
        [RelayCommand]
        public async Task ImprimirAsync()
        {
            if (_ultimoTicket == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "No hay ticket para imprimir.", "OK");
                return;
            }

            await ImprimirTicketAsync(_ultimoTicket);
        }
    }
}
