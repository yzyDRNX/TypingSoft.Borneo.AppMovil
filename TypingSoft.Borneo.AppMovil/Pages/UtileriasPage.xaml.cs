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
    VModels.CatalogosVM ViewModel => this.BindingContext as VModels.CatalogosVM;
    private async void Impresion(object sender, EventArgs e)
    {
        var vm = BindingContext as UtileriasPageViewModel;
        //if (vm?.VentaActual == null)
        //{
        //    await DisplayAlert("Error", "No hay venta para imprimir.", "OK");
        //    return;
        //}

        // Genera el texto del ticket usando tu clase de formato
        string ticket = TicketFormatter.FormatearTicket(vm.VentaActual);

        // Obtén el servicio de impresión
        var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
        if (printer != null)
            await printer.PrintTextAsync(ticket);
        else
            await DisplayAlert("Error", "No se encontró el servicio de impresión.", "OK");
    }


    //private async void Impresion(object sender, EventArgs e)
    //{
    //    var vm = BindingContext as UtileriasPageViewModel;
    //    if (vm?.VentaActual == null)
    //    {
    //        await DisplayAlert("Error", "No hay venta para imprimir.", "OK");
    //        return;
    //    }

    //    string ticket = TypingSoft.Borneo.AppMovil.Helpers.TicketFormatter.FormatearTicket(vm.VentaActual);

    //    // Obtén el servicio de impresión
    //    var printer = App.ServiceProvider.GetService<IRawBtPrinter>();
    //    if (printer != null)
    //        await printer.PrintTextAsync(ticket);
    //    else
    //        await DisplayAlert("Error", "No se encontró el servicio de impresión.", "OK");
    //}
}