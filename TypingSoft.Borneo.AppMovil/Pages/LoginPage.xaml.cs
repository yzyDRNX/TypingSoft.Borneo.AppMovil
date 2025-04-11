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
            // Verificar permiso de cámara
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permiso denegado", "No se puede acceder a la cámara.", "OK");
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
                    await DisplayAlert("Procesando", "Analizando código QR...", "OK");

                    // Aquí normalmente procesarías la imagen para detectar el código QR
                    // Como esto requeriría una biblioteca adicional, simularemos el resultado

                    // Simulación de lectura QR exitosa
                    string qrContent = "usuario:123456"; // En un caso real, esto vendría del procesamiento de la imagen

                    // Procesar el resultado
                    bool isValid = ValidateQRContent(qrContent);

                    if (isValid)
                    {
                        // Navegar a la página principal después de login exitoso
                        await App.NavigationService.Navegar(nameof(EmpleadosPage));
                    }
                    else
                    {
                        await DisplayAlert("Error", "Código QR inválido o no reconocido.", "OK");
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
            // Implementa tu lógica de validación aquí
            // Por ejemplo, verificar que tiene el formato correcto
            // o que contiene información válida para autenticación

            // Ejemplo simple de validación
            return !string.IsNullOrEmpty(content) && content.Contains(":");
        }
    }
}