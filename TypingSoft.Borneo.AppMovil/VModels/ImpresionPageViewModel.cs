using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Text;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Pages;
using TypingSoft.Borneo.AppMovil.Helpers;


namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class ImpresionPageViewModel : Helpers.VMBase
    {
        [ObservableProperty]
        private VentaGeneralRequestDTO venta;

        public ImpresionPageViewModel(VentaGeneralRequestDTO venta)
        {
            this.venta = venta;
        }

        // Propiedad para mostrar el ticket en la vista previa
        public string TicketPreview => TicketFormatter.FormatearTicket(venta);

        // Comando para imprimir
        [RelayCommand]
        private async Task ImprimirAsync()
        {
            string ticket = TicketFormatter.FormatearTicket(venta);
            // Aquí llamas a tu servicio de impresión
            // await MiniPrinterService.ImprimirAsync(ticket);
        }
    }
}
