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
        if (vm != null)
            await vm.ImprimirAsync();
    }
}