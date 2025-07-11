namespace TypingSoft.Borneo.AppMovil.Pages;
using Shiny.BluetoothLE;
using System.Text;
using TypingSoft.Borneo.AppMovil.VModels;
using TypingSoft.Borneo.AppMovil.Helpers;
using TypingSoft.Borneo.AppMovil.Local;

public partial class UtileriasPage : ContentPage
{
    public string FechaGuardada { get; private set; }
    IBleManager bleManager;

    public UtileriasPage()
    {
        InitializeComponent();
        var vm = App.ServiceProvider.GetService<UtileriasPageViewModel>();
        BindingContext = vm;
        FechaGuardada = vm?.FechaActual ?? "";
        bleManager = App.ServiceProvider.GetService<IBleManager>();

        // Cargar datos de la venta actual y productos
        if (vm != null)
            _ = vm.CargarVentaActualYProductos();
    }

    private async void Impresion(object sender, EventArgs e)
    {
        var vm = BindingContext as UtileriasPageViewModel;
        if (vm != null)
            await vm.ImprimirAsync();
    }

    private async void OtraVentaMismoCliente(object sender, EventArgs e)
    {
        var vm = BindingContext as UtileriasPageViewModel;
        if (vm != null)
            await vm.OtraVentaMismoClienteAsync();
    }

    private async void SiguienteEntrega(object sender, EventArgs e)
    {
        var vm = BindingContext as UtileriasPageViewModel;
        if (vm != null)
            await vm.SiguienteEntregaAsync();
    }
}