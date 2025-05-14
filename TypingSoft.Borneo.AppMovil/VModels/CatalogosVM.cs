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



        #endregion

        #region Métodos
        public async void ObtenerEmpleados()
        {
            this.MensajeProcesando = "Cargando Empleados";
            this.Procesando = true;

            var (exitoso, mensaje, lista) = await this.Catalogos.ObtenerEmpleados();

            if (exitoso)
            {
                this.ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(lista);
            }
            else
            {
                var errormsg = "Ocurrió un error en la petición";
            }

            this.Procesando = false;
        }

        public async Task ObtenerClientes()
        {
            this.MensajeProcesando = "Cargando Clientes";
            this.Procesando = true;

            

            Guid idRuta = Helpers.Settings.IdRuta;
            var (exitoso, mensaje, lista) = await this.Catalogos.ObtenerClientes(idRuta);

            if (exitoso)
            {
                this.ListadoClientes = new ObservableCollection<Models.Custom.ClientesLista>(lista);
            }
            else
            {
                var errormsg = "Ocurrió un error en la petición";
            }

            this.Procesando = false;
        }




        #endregion

        #region Comandos

        #endregion
    }
}
