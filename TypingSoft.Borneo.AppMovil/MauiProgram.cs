using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace TypingSoft.Borneo.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseBarcodeReader(); // Añade esta línea para registrar el servicio

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<Helpers.CustomNavigation>();

            #region Bussiness Logic
            builder.Services.AddSingleton<BL.CatalogosBL>();
            #endregion

            #region Services
            builder.Services.AddSingleton<Services.CatalogosService>();
            #endregion

            #region Vmodels
            builder.Services.AddSingleton<VModels.CatalogosVM>();
            #endregion


            var app = builder.Build();
            // Asignar el proveedor de servicios a la propiedad estática en App
            App.ServiceProvider = app.Services;

            return builder.Build();
        }
    }
}