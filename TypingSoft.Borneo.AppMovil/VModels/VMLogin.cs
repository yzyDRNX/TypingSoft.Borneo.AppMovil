using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Services;


namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class LoginVM : Helpers.VMBase
    {
        #region Constructor
        private readonly BL.Security _seguridad;
        private readonly Services.LocalDatabaseService _localDb;
        private readonly Services.SincronizacionService _sincronizacion;

        public LoginVM(BL.Security seguridad, LocalDatabaseService localDb, SincronizacionService sincronizacion)
        {
            _seguridad = seguridad;
            _localDb = localDb;
            _sincronizacion = sincronizacion;
        }

        #endregion

        #region Propiedades Login
        [ObservableProperty]
        string ruta;


        #endregion


        #region Comandos
        [RelayCommand]
        public async Task AutenticarRuta()
        {
            this.MensajeProcesando = "Verificando información";
            this.Procesando = true;

            try
            {
                (bool autenticado, string mensaje, Models.API.RutaResponse.Rutas? rutaObj) = await this._seguridad.AutenticarRuta(this.Ruta);
                if (autenticado && rutaObj != null)
                {
                    // Guardar en SQLite
                    var rutaLocal = new Local.RutaLocal
                    {
                        Id = Guid.NewGuid(),
                        Descripcion = rutaObj.Descripcion
                    };
                    await _localDb.GuardarRutaAsync(rutaLocal);

                    Helpers.Settings.IdRuta = rutaObj.Id;
                    Helpers.Settings.UltimaDescripcionRuta = rutaObj.Descripcion;

                    // Sincroniza todos los catálogos aquí
                    MensajeProcesando = "Sincronizando catálogos...";
                    await _sincronizacion.SincronizarCatalogosAsync();

                    await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
                    MensajeProcesando = mensaje;
                    return;
                }
                else
                {
                    // Si no se pudo autenticar, intenta cargar la ruta local
                    var rutaLocal = await _localDb.ObtenerRutaAsync();
                    if (rutaLocal != null && !string.IsNullOrEmpty(rutaLocal.Descripcion))
                    {
                        Helpers.Settings.UltimaDescripcionRuta = rutaLocal.Descripcion;
                        await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
                        MensajeProcesando = "Modo sin conexión: usando ruta local.";
                        return;
                    }
                    else
                    {
                        MensajeProcesando = "No se pudo autenticar y no hay datos locales.";
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de excepción, intenta cargar la ruta local
                var rutaLocal = await _localDb.ObtenerRutaAsync();
                if (rutaLocal != null && !string.IsNullOrEmpty(rutaLocal.Descripcion))
                {
                    Helpers.Settings.UltimaDescripcionRuta = rutaLocal.Descripcion;
                    await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
                    MensajeProcesando = "Modo sin conexión: usando ruta local.";
                    return;
                }
                MensajeProcesando = "Error al llamar al servicio y no hay datos locales: " + ex.Message;
            }
        }


        #endregion 


    }
}
