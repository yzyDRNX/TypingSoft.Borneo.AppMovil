using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class LoginVM : Helpers.VMBase
    {
        #region Constructor
        private readonly BL.Security _seguridad;
        public LoginVM(BL.Security seguridad)
        {
            _seguridad = seguridad;
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
                if (autenticado == true && rutaObj != null)
                {
                    Helpers.Settings.IdRuta = rutaObj.Id;
                    Helpers.Settings.UltimaDescripcionRuta = rutaObj.Descripcion;

                    MensajeProcesando = mensaje;
                    await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
                    return;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("=== Exception en IniciarSesion ===");
                Debug.WriteLine(ex.ToString());
                MensajeProcesando = "Error al llamar al servicio: " + ex.Message;
            }
        }
        #endregion 


    }
}
