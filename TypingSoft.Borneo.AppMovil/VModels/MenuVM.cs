using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class MenuVM : Helpers.VMBase
    {

        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;
        private readonly Services.SincronizacionService _sincronizacion;
        private readonly SincronizacionVentasService _sincronizacionVentas;

        public MenuVM(
            Services.SincronizacionService sincronizacion,
            BL.CatalogosBL catalogos,
            LocalDatabaseService localDb,
            SincronizacionVentasService sincronizacionVentas
        )
        {
            _catalogos = catalogos;
            _localDb = localDb;
            _sincronizacion = sincronizacion;
            _sincronizacionVentas = sincronizacionVentas;
            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }


        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
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

        [RelayCommand]
        public async Task SincronizarVentas()
        {
            this.MensajeProcesando = "Sincronizando ventas...";
            this.Procesando = true;
            try
            {
                await _sincronizacionVentas.SincronizarVentasYDetallesAsync();
                this.MensajeProcesando = "Ventas sincronizadas correctamente.";
            }
            catch (Exception ex)
            {
                this.MensajeProcesando = $"Error al sincronizar ventas: {ex.Message}";
            }
            finally
            {
                this.Procesando = false;
            }
        }
    }
}
