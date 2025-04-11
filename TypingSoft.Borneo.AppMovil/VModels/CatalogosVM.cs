using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class CatalogosVM : Helpers.VMBase
    {
        #region Constructor
        BL.CatalogosBL Catalogos;
        public CatalogosVM(BL.CatalogosBL catalogos)
        {
            this.Catalogos = catalogos;
            this.ListadoEmpleados = new();
        }
        #endregion

        #region Propiedades
        [ObservableProperty]
        List<Models.Custom.EmpleadosLista> listadoEmpleados;

       
        #endregion

        #region Métodos
        public async void ObtenerEmpleados()
        {
            this.MensajeProcesando = "Cargando Empleados";
            this.Procesando = true;
            var lista = await this.Catalogos.ObtenerEmpleados();
            this.ListadoEmpleados = lista.Empleados;

        
            this.Procesando = false;
        }
        #endregion

        #region Comandos

        #endregion
    }
}
