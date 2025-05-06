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
        async Task AutenticarRuta()
        {
            this.MensajeProcesando = "Verificando información";
            this.Procesando = true;

            try
            {
                var (autenticado, mensaje, rutaObj) = await this._seguridad.AutenticarRuta(this.Ruta);
                if (!autenticado || rutaObj == null)
                {
                    MensajeProcesando = mensaje;
                    return;
                }
                await Navegacion.Navegar(nameof(Pages.EmpleadosPage));
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
