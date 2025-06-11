using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using TypingSoft.Borneo.AppMovil.Models.Custom;
using TypingSoft.Borneo.AppMovil.Services;

namespace TypingSoft.Borneo.AppMovil.VModels
{
    public partial class EmpleadosVM : Helpers.VMBase
    {
        private readonly BL.CatalogosBL _catalogos;
        public readonly LocalDatabaseService _localDb;


        public EmpleadosVM(BL.CatalogosBL catalogos, LocalDatabaseService localDb)
        {
            _catalogos = catalogos;
            _localDb = localDb;

            ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>();

            fechaActual = DateTime.Now.ToString("dd-MM-yyyy");
            _ = CargarDescripcionRutaAsync();
        }
        [ObservableProperty]
        string fechaActual;

        [ObservableProperty]
        string descripcionRuta;
        [ObservableProperty]
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;


        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            System.Diagnostics.Debug.WriteLine($"DescripcionRuta cargada: {descripcion}");
            DescripcionRuta = descripcion;
        }


        public async Task CargarEmpleadosDesdeLocal()
        {
            var empleadosLocales = await _localDb.ObtenerEmpleadosAsync();

            if (empleadosLocales != null && empleadosLocales.Any())
            {
                ListadoEmpleados = new ObservableCollection<Models.Custom.EmpleadosLista>(
                    empleadosLocales.Select(e => new Models.Custom.EmpleadosLista
                    {
                        Id = e.Id,
                        Empleado = e.Empleado
                        // Ajusta aquí si usas más campos como Apellidos, etc.
                    })
                );
                await MostrarAlertaAsync("Modo sin conexión", "Mostrando empleados locales.");
            }
            else
            {
                //await MostrarAlertaAsync("Error", mensajeError + "\nNo hay datos locales disponibles.");
            }
        }



        private async void CargarDescripcionRuta()
        {
            var db = new LocalDatabaseService();
            DescripcionRuta = await db.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
        }
        #region Alerta 
        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
        #endregion

    }

}
