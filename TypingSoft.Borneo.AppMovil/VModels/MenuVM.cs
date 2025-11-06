using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
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

        // Propiedades para mostrar modales de éxito
        [ObservableProperty]
        bool mostrarExitoDescarga;

        [ObservableProperty]
        bool mostrarExitoSincronizacion;

        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }

        [RelayCommand]
        public async Task SincronizarCatalogos()
        {
            // Asegurar que los modales previos estén ocultos
            MostrarExitoDescarga = false;
            MostrarExitoSincronizacion = false;

            this.MensajeProcesando = "Sincronizando catálogos...";
            this.Procesando = true;
            var exito = false;
            try
            {
                await _sincronizacion.SincronizarCatalogosAsync();
                exito = true;
                this.MensajeProcesando = "Catálogos sincronizados correctamente.";
            }
            catch (Exception ex)
            {
                this.MensajeProcesando = $"Error al sincronizar: {ex.Message}";
            }
            finally
            {
                this.Procesando = false;
                if (exito)
                    MostrarExitoDescarga = true;
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
                await Navegacion.Navegar(nameof(Pages.LoginPage));
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
            MostrarExitoDescarga = false;
            MostrarExitoSincronizacion = false;

            this.MensajeProcesando = "Sincronizando ventas...";
            this.Procesando = true;
            var exito = false;
            try
            {
               // await _localDb.ImprimirVentasDebugAsync();

                // 1) Subir Ventas + Detalles
                await _sincronizacionVentas.SincronizarVentasYDetallesAsync();

                //await _localDb.ImprimirVentasDebugAsync();

                // 2) Subir folio actualizado (manual al presionar este botón)
                var valoresBL = new BL.ValoresAppVentaDetalleBL(_localDb, new Services.ValoresAppVentaDetalleService());
                var okFolio = await valoresBL.ActualizarFolioServidorDesdeLocalAsync();
                //System.Diagnostics.Debug.WriteLine($"[SYNC][Folio->API] {(okFolio ? "OK" : "NO-OP")}");

                exito = true;
                this.MensajeProcesando = "Ventas y folio sincronizados correctamente.";
            }
            catch (Exception ex)
            {
                this.MensajeProcesando = $"Error al sincronizar ventas: {ex.Message}";
            }
            finally
            {
                this.Procesando = false;
                if (exito)
                    MostrarExitoSincronizacion = true;
            }
        }

        [RelayCommand]
        void CerrarExitoDescarga() => MostrarExitoDescarga = false;

        [RelayCommand]
        void CerrarExitoSincronizacion() => MostrarExitoSincronizacion = false;
    }
}
