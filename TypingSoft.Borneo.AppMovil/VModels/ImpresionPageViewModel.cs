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

        public ImpresionPageViewModel(VentaGeneralRequestDTO venta, string fechaString)
        {
            this.venta = venta;

            // Asigna la fecha a cada venta general en el DTO
            if (DateTime.TryParseExact(fechaString, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var fecha))
            {
                foreach (var item in venta.Data)
                {
                    item.Fecha = fecha;
                }
            }
        }

        // Propiedad para mostrar el ticket en la vista previa
        public string TicketPreview => TicketFormatter.FormatearTicket(venta);

        // Comando para imprimir
        [RelayCommand]
        private async Task ImprimirAsync(string fechaString)
        {
            if (DateTime.TryParseExact(fechaString, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var fecha))
            {
                foreach (var item in venta.Data)
                {
                    item.Fecha = fecha;
                }
            }

            string ticket = TicketFormatter.FormatearTicket(venta);
            // await MiniPrinterService.ImprimirAsync(ticket);
        }

    }

}
