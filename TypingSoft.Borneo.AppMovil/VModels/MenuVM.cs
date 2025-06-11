using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class MenuVM:Helpers.VMBase
    {


        private readonly Services.SincronizacionService _sincronizacion;

        public MenuVM(Services.SincronizacionService sincronizacion)
        {
            _sincronizacion = sincronizacion;
        }

        [RelayCommand]
        public async Task SincronizarCatalogos()
        {
            this.MensajeProcesando = "Sincronizando catálogos...";
            this.Procesando = true;
            try
            {
                await _sincronizacion.SincronizarCatalogosAsync();
                this.MensajeProcesando = "Catálogos sincronizados correctamente.";
            }
            catch (Exception ex)
            {
                this.MensajeProcesando = $"Error al sincronizar: {ex.Message}";
            }
            finally
            {
                this.Procesando = false;
            }
        }

        [RelayCommand]
        public async Task NavegarInicio()
        {
            this.MensajeProcesando = "Navegando al inicio...";
            this.Procesando = true;
            try
            {
                await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
            }
            catch (Exception ex)
            {
                this.MensajeError = $"Error al navegar: {ex.Message}";
                this.ExisteError = true;
            }
            finally
            {
                this.Procesando = false;
            }
        }
        [RelayCommand]
        public async Task CerrarSesion()
        {
            this.MensajeProcesando = "Cerrando sesión...";
            this.Procesando = true;
            try
            {
                // Aquí puedes agregar la lógica para cerrar sesión, si es necesario.
                await Navegacion.Navegar(nameof(Pages.LoginPage));
                // Limpiar la ruta y otros datos si es necesario
         
                Helpers.Settings.UltimaDescripcionRuta = string.Empty;
                this.MensajeProcesando = "Sesión cerrada correctamente.";
                Helpers.StaticSettings.LimpiarSesion();
            }
            catch (Exception ex)
            {
                this.MensajeError = $"Error al cerrar sesión: {ex.Message}";
                this.ExisteError = true;
            }
            finally
            {
                this.Procesando = false;
            }
        }
    }
}
