using ZXing.Net.Maui;
namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class LoginPage : ContentPage
{
    VModels.LoginVM ViewModel => this.BindingContext as VModels.LoginVM;
    public LoginPage()
    {
        InitializeComponent();
        var viewModel = App.ServiceProvider.GetService<VModels.LoginVM>();
        if (viewModel != null)
            this.BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //this.ViewModel.Ruta = string.Empty;
   
    }
protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
{
    // Procesar solo el primer c�digo detectado
    var firstBarcode = e.Results.FirstOrDefault();
    if (firstBarcode != null)
    {
        Console.WriteLine($"Barcodes: {firstBarcode.Format} -> {firstBarcode.Value}");

        // Mostrar un mensaje al usuario
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("C�digo QR le�do", $"Valor: {firstBarcode.Value}","Es su Ruta?", "OK");
            await ViewModel.AutenticarRuta();
        });

        // Asignar el valor al ViewModel
        ViewModel.Ruta = firstBarcode.Value;

        // Detener la c�mara despu�s de leer el primer c�digo
        StopCamera();
    }
}




    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Solicitar permisos de c�mara
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status == PermissionStatus.Granted)
        {
            // Activa la c�mara al hacer visible el control
            cameraBarcodeReaderView.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Permiso denegado", "Se requiere acceso a la c�mara para escanear el c�digo QR.", "OK");
        }
    }
    private void StopCamera()
    {
        // Ocultar el control para detener la c�mara
        cameraBarcodeReaderView.IsVisible = false;

        // Desactivar el evento para evitar lecturas adicionales
        cameraBarcodeReaderView.BarcodesDetected -= BarcodesDetected;

        // Si el control tiene un m�todo para liberar recursos, ll�malo aqu�
        // Esto depende de la implementaci�n de ZXing.Net.Maui
    }


}