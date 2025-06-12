namespace TypingSoft.Borneo.AppMovil.Pages;
using Shiny.BluetoothLE;
using System.Text;
using TypingSoft.Borneo.AppMovil.VModels;
using TypingSoft.Borneo.AppMovil.Helpers;

public partial class UtileriasPage : ContentPage
{
    public string FechaGuardada { get; private set; }
    IBleManager bleManager;

    public UtileriasPage()
    {
        InitializeComponent();
        var vm = new UtileriasPageViewModel();
        BindingContext = vm;
        FechaGuardada = vm.FechaActual;
        bleManager = App.ServiceProvider.GetService<IBleManager>();
    }

    private async void Impresion(object sender, EventArgs e)
    {
        var vm = BindingContext as UtileriasPageViewModel;
        //if (vm?.VentaActual == null)
        //{
        //    await DisplayAlert("Error", "No hay venta para imprimir.", "OK");
        //    return;
        //}

        string ticket = TicketFormatter.FormatearTicket(vm.VentaActual);

        // Obt�n el servicio de impresi�n
        var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
        if (printer != null)
            await printer.PrintTextAsync(ticket);
        else
            await DisplayAlert("Error", "No se encontr� el servicio de impresi�n.", "OK");
    }
}