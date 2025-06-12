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
    public partial class ImpresionPageViewModel : Helpers.VMBase
    {


        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;


        public ImpresionPageViewModel(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }

        

        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;
        [ObservableProperty]
        private VentaGeneralResponse venta;

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }

        public ImpresionPageViewModel(VentaGeneralResponse venta, string fechaString)
        {
            this.venta = venta;

            if (DateTime.TryParseExact(fechaString, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var fecha))
            {
                foreach (var item in venta.Data)
                {
                    item.Fecha = fecha;
                }
            }
        }

        public string TicketPreview => TicketFormatter.FormatearTicket(venta);

        [RelayCommand]
        private async Task ImprimirAsync()
        {
            if (venta == null || venta.Data == null || venta.Data.Count == 0)
                return;

            string ticket = TicketFormatter.FormatearTicket(venta);

            var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
            if (printer != null)
                await printer.PrintTextAsync(ticket);
        }
    }
}
