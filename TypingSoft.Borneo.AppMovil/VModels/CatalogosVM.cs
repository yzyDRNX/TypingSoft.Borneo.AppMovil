using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TypingSoft.Borneo.AppMovil;

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
            this.ListadoClientes = new();
        }
        #endregion

        #region Propiedades
        [ObservableProperty]
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;

        [ObservableProperty]
        ObservableCollection<Models.Custom.ClientesLista> listadoClientes;

        public CatalogosVM(BL.CatalogosBL catalogos, Services.LocalDatabaseService localDb)
        {
            this.Catalogos = catalogos;
            this.localDb = localDb;
            this.ListadoEmpleados = new();
        }

        #endregion

        #region Métodos
        public async void ObtenerEmpleados()
        {
            try
            {
                this.MensajeProcesando = "Cargando Empleados";
                this.Procesando = true;

            var lista = await this.Catalogos.ObtenerEmpleados(); // Este llama a la API SQL Server
            this.ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(lista.Empleados);



            this.Procesando = false;
        }


        #endregion

        #region Comandos

        #endregion
    }
}
