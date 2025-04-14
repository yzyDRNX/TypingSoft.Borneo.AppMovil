using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class LoginVM : Helpers.VMBase
    {
        #region Constructor
        BL.Security Seguridad;
        public LoginVM(BL.Security seguridad)
        {
            Seguridad = seguridad;
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
            var autenticado = await this.Seguridad.AutenticarRuta(this.Ruta);
            if (autenticado.Autenticado)
            {
                var infoRuta = await this.Seguridad.ObtenerInformacionRuta();
                if (infoRuta.Exitoso)
                    await this.Navegacion.Navegar(nameof(Pages.EmpleadosPage));
                else
                    //await this.Navegacion.MostrarMopup(nameof(Mopups.IconMessage), new object[] { infoUsuario.Mensaje, "Ok", Helpers.TipoMopup.Error });
                    Console.WriteLine("Error");

            }
            else
                //await this.Navegacion.MostrarMopup(nameof(Mopups.IconMessage), new object[] { autenticado.Mensaje, "Aceptar", Helpers.TipoMopup.Error });
            this.Procesando = false;
        }




        #endregion
    }
}
