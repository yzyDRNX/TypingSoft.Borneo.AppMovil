using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;
       
        Services.LocalDatabaseService localDb;

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

                // Obtener desde API
                var respuesta = await this.Catalogos.ObtenerEmpleados();
                var empleadosApi = respuesta.Empleados;

                // Asignar a la propiedad observable para la UI
                this.ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(empleadosApi);

                // Convertir a EmpleadoLocal y guardar en SQLite
                var empleadosLocales = empleadosApi.Select(e => new Local.EmpleadoLocal
                {
                    Id = e.Id,
                    Nombre = e.Empleado
                }).ToList();

                await localDb.GuardarEmpleadosAsync(empleadosLocales);
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
            finally
            {
                this.Procesando = false;
            }
        }



        #endregion

        #region Comandos

        #endregion
    }
}
