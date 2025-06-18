//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using System;
//using System.Threading.Tasks;
//using TypingSoft.Borneo.AppMovil.Helpers;
//using Microsoft.Extensions.DependencyInjection;
//using TypingSoft.Borneo.AppMovil.Services;
//using TypingSoft.Borneo.AppMovil.Local;
//using System.Linq;

//namespace TypingSoft.Borneo.AppMovil.VModels
//{
//    public partial class ImpresionPageViewModel : Helpers.VMBase
//    {
//        private readonly LocalDatabaseService _localDb;
//        private int _numeroImpresiones = 0;
//        private TicketLocal _ultimoTicket;

//        public ImpresionPageViewModel(LocalDatabaseService localDb)
//        {
//            _localDb = localDb;
//            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
//            _ = CargarUltimoTicketAsync();
//        }

//        [ObservableProperty]
//        string fechaActual;

//        [ObservableProperty]
//        string descripcionRuta;

//        [ObservableProperty]
//        string ticketPreview;

//        private async Task CargarUltimoTicketAsync()
//        {
//            var tickets = await _localDb.ObtenerTicketsAsync();
//            _ultimoTicket = tickets?.OrderByDescending(t => t.Fecha).FirstOrDefault();

//            if (_ultimoTicket != null)
//            {
//                TicketPreview = TicketFormatter.FormatearTicketLocal(_ultimoTicket, _numeroImpresiones + 1);
//            }
//            else
//            {
//                TicketPreview = "No hay ticket para imprimir.";
//            }
//        }

//        [RelayCommand]
//        private async Task ImprimirAsync()
//        {
//            if (_ultimoTicket == null)
//                return;

//            _numeroImpresiones++;
//            string ticket = TicketFormatter.FormatearTicketLocal(_ultimoTicket, _numeroImpresiones);

//            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
//            if (printer != null)
//                await printer.PrintTextAsync(ticket);

//            // Actualiza el preview para reflejar el tipo de copia
//            TicketPreview = ticket;
//        }
//    }
//}
