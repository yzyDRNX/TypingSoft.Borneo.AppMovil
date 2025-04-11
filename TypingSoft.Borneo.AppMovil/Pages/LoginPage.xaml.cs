using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Media;

namespace TypingSoft.Borneo.AppMovil.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnScanQRClicked(object sender, EventArgs e)
        {
            // Verificar permiso de c�mara
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permiso denegado", "No se puede acceder a la c�mara.", "OK");
                    return;
                }
            }

            try
            {
                // Usar MediaPicker para capturar una foto
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    // Mostrar indicador de procesamiento
                    await DisplayAlert("Procesando", "Analizando c�digo QR...", "OK");

                    // Aqu� normalmente procesar�as la imagen para detectar el c�digo QR
                    // Como esto requerir�a una biblioteca adicional, simularemos el resultado

                    // Simulaci�n de lectura QR exitosa
                    string qrContent = "usuario:123456"; // En un caso real, esto vendr�a del procesamiento de la imagen

                    // Procesar el resultado
                    bool isValid = ValidateQRContent(qrContent);

                    if (isValid)
                    {
                        // Navegar a la p�gina principal despu�s de login exitoso
                        await App.NavigationService.Navegar(nameof(EmpleadosPage));
                    }
                    else
                    {
                        await DisplayAlert("Error", "C�digo QR inv�lido o no reconocido.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private bool ValidateQRContent(string content)
        {
            // Implementa tu l�gica de validaci�n aqu�
            // Por ejemplo, verificar que tiene el formato correcto
            // o que contiene informaci�n v�lida para autenticaci�n

            // Ejemplo simple de validaci�n
            return !string.IsNullOrEmpty(content) && content.Contains(":");
        }
    }
}