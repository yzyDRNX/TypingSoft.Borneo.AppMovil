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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        cameraBarcodeReaderView.Opacity = 0;
        await cameraBarcodeReaderView.FadeTo(1, 600, Easing.CubicIn);
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status == PermissionStatus.Granted)
        {
            cameraBarcodeReaderView.IsVisible = true;
        }
        else
        {
            await DisplayAlert("Permiso denegado", "Se requiere acceso a la cámara para escanear el código QR.", "OK");
        }
    }

    private void StopCamera()
    {
        cameraBarcodeReaderView.IsVisible = false;
        cameraBarcodeReaderView.BarcodesDetected -= BarcodesDetected;
    }

    private async void OnPageAppearing(object sender, EventArgs e)
    {
        MainLayout.Opacity = 0;
        MainLayout.Scale = 0.8;

        await MainLayout.FadeTo(1, 500, Easing.CubicInOut);
        await MainLayout.ScaleTo(1, 300, Easing.CubicOut);
    }

    protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var firstBarcode = e.Results.FirstOrDefault();
        if (firstBarcode != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ViewModel.Ruta = firstBarcode.Value;

                bool autenticado = false;
                await MostrarModalCarga(async () =>
                {
                    await ViewModel.AutenticarRuta();
                    // Verifica si la autenticación fue exitosa
                    autenticado = ViewModel.Procesando == false && string.IsNullOrEmpty(ViewModel.MensajeError) && !ViewModel.ExisteError
                        && (ViewModel.MensajeProcesando?.Contains("Sincronizando") == true || ViewModel.MensajeProcesando?.Contains("Ruta aceptada") == true);
                });

                if (autenticado)
                {
                    StopCamera();
                }
                else
                {
                    // Si no fue autenticado, la cámara sigue activa para reintentar
                    await DisplayAlert("Error", "No se pudo autenticar la ruta. Intenta escanear nuevamente.", "OK");
                    cameraBarcodeReaderView.IsVisible = true;
                    cameraBarcodeReaderView.BarcodesDetected += BarcodesDetected;
                }
            });

            StopCamera();
        }
    }
    private async void OnManualLoginClicked(object sender, EventArgs e)
    {
        string ruta = ViewModel?.Ruta?.Trim();

        await MostrarModalCarga(async () =>
        {
            // Mostrar "Verificando..." mientras se ejecuta
            LoadingLabel.Text = "Verificando...";
            LoadingIndicator.IsRunning = true;

            // Ejecutar el comando original sin lógica adicional
            if (ViewModel.AutenticarRutaCommand?.CanExecute(null) == true)
            {
                ViewModel.AutenticarRutaCommand.Execute(null);

                // Esperar un poco por estética
                await Task.Delay(1000);

                LoadingLabel.Text = "Ruta aceptada ✅";
            }
            else
            {
                await Task.Delay(1000);
                LoadingLabel.Text = "Ruta inválida ❌";
            }

            LoadingIndicator.IsRunning = false;
            await Task.Delay(1500); // Mostrar resultado antes de cerrar
        });
    }




    private async Task MostrarModalCarga(Func<Task> accion)
    {
        LoadingOverlay.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        LoadingLabel.Text = "Verificando...";

        try
        {
            await accion.Invoke();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            await Task.Delay(500);
            LoadingOverlay.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}
