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
    // Procesar solo el primer código detectado
    var firstBarcode = e.Results.FirstOrDefault();
    if (firstBarcode != null)
    {
        Console.WriteLine($"Barcodes: {firstBarcode.Format} -> {firstBarcode.Value}");

        // Mostrar un mensaje al usuario
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Código QR leído", $"Valor: {firstBarcode.Value}","Es su Ruta?", "OK");
            await ViewModel.AutenticarRuta();
        });

        // Asignar el valor al ViewModel
        ViewModel.Ruta = firstBarcode.Value;

        // Detener la cámara después de leer el primer código
        StopCamera();
    }
}




    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Solicitar permisos de cámara
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status == PermissionStatus.Granted)
        {
            // Activa la cámara al hacer visible el control
            cameraBarcodeReaderView.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Permiso denegado", "Se requiere acceso a la cámara para escanear el código QR.", "OK");
        }
    }
    private void StopCamera()
    {
        // Ocultar el control para detener la cámara
        cameraBarcodeReaderView.IsVisible = false;

        // Desactivar el evento para evitar lecturas adicionales
        cameraBarcodeReaderView.BarcodesDetected -= BarcodesDetected;

        // Si el control tiene un método para liberar recursos, llámalo aquí
        // Esto depende de la implementación de ZXing.Net.Maui
    }


}