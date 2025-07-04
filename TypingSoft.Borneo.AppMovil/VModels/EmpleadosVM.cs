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
        private Guid idRutaActual;

        [ObservableProperty]
        ObservableCollection<Models.Custom.EmpleadosLista> listadoEmpleados;


        private async Task CargarDescripcionRutaAsync()
        {
            var descripcion = await _localDb.ObtenerDescripcionRutaAsync() ?? "Sin descripción";
            DescripcionRuta = descripcion;

            // Aquí recuperas el Guid de la ruta (puede ser nulo)
            Guid? idRuta = await _localDb.ObtenerIdRutaAsync();

            if (idRuta.HasValue)
            {
                IdRutaActual = idRuta.Value;
            }
            else
            {
                // Maneja el caso en que no se pudo obtener el IdRuta
                await MostrarAlertaAsync("Error", "No se pudo obtener el Id de la ruta.");
                IdRutaActual = Guid.Empty; // o no lo asignas, según tu lógica
            }
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


        #region Alerta 
        private Task MostrarAlertaAsync(string titulo, string mensaje)
        {
            return Task.CompletedTask;
        }
        #endregion

        [RelayCommand]
        public async Task VolverMenu()
        {
            // Navega a la página de menú y limpia la pila de navegación
            await Navegacion.Navegar(nameof(Pages.MenuPage));
        }
    }

}
