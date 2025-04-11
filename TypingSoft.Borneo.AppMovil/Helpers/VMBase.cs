using CommunityToolkit.Mvvm.ComponentModel;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public partial class VMBase : ObservableObject
    {
        #region Constructor
        public Helpers.CustomNavigation Navegacion => Helpers.GlobalValues.NavegacionGlobal;
        public VMBase()
        {
            this.MensajeProcesando = this.MensajeError = string.Empty;
        }
        #endregion

        #region Propiedades
        [ObservableProperty]
        bool procesando;

        [ObservableProperty]
        string mensajeProcesando;

        [ObservableProperty]
        string mensajeError;
        [ObservableProperty]
        bool existeError;
        #endregion
    }
}
