using Microsoft.Maui.Controls;
using System;

namespace TypingSoft.Borneo.AppMovil.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnScanQRClicked(object sender, EventArgs e)
    {
        // Esto es solo representativo, no funcionará realmente
        // pero muestra cómo se podría estructurar el código para abrir la cámara


        
            // Aquí iría el código real para abrir la cámara y escanear QR
            await DisplayAlert("Cámara", "Abriendo escáner de QR...", "OK");

            // En una implementación real, aquí llamarías a un servicio de escaneo QR
            // y luego procesarías el resultado

            // Ejemplo de navegación después de un escaneo exitoso (simulado):
            // await Navigation.PushAsync(new RecorridoOrdinarioPage());
            // Usar CustomNavigation para navegar a ClientePage
            await App.NavigationService.Navegar(nameof(EmpleadosPage));
        
       
    }

}