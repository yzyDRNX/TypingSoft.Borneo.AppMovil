using Microsoft.Extensions.Logging;
using TypingSoft.Borneo.AppMovil.BL;
using TypingSoft.Borneo.AppMovil.Helpers;
using TypingSoft.Borneo.AppMovil.Services;
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
                .UseBarcodeReader();

#if ANDROID
            builder.Services.AddSingleton<IRawBtPrinter, RawBtPrinter>();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<Helpers.CustomNavigation>();
            builder.Services.AddSingleton<VentaGeneralService>();
            builder.Services.AddSingleton<VentaGeneralBL>();
            builder.Services.AddSingleton<VentaDetalleService>();
            builder.Services.AddSingleton<DetalleVentaBL>();
            builder.Services.AddSingleton<LocalDatabaseService>();
            builder.Services.AddSingleton<SincronizacionVentasService>();

            RegisterBusinessLogic(builder.Services);
            RegisterServices(builder.Services);
            RegisterViewModels(builder.Services);

            var app = builder.Build();
            App.ServiceProvider = app.Services;
            return app;
        }

        private static void RegisterBusinessLogic(IServiceCollection services)
        {
            services.AddSingleton<BL.CatalogosBL>();
            services.AddSingleton<BL.Security>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<Services.CatalogosService>();
            services.AddSingleton<VentaSessionServices>();
            services.AddSingleton<Services.SeguridadService>();
            services.AddSingleton<Services.LocalDBService>();
            services.AddSingleton<Services.LocalDatabaseService>();
            services.AddSingleton<SincronizacionService>();
            services.AddSingleton<SincronizacionVentasService>();
            services.AddSingleton<Services.VentaDetalleService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddSingleton<VModels.CatalogosVM>();
            services.AddSingleton<VModels.LoginVM>();
            services.AddSingleton<VModels.MenuVM>();
            services.AddSingleton<VModels.EmpleadosVM>();
            services.AddSingleton<VModels.RepartoVM>();
            services.AddSingleton<VModels.UtileriasPageViewModel>();
            services.AddSingleton<VModels.ClientePageViewModel>();
            services.AddSingleton<VModels.UrlPageVM>();

        }
    }
}
