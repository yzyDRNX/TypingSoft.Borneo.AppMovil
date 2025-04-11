using Microsoft.Extensions.Logging;

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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<Helpers.CustomNavigation>();

            #region Bussiness Logic
            builder.Services.AddSingleton<BL.AhorroBL>();

            #endregion

            #region Services
            builder.Services.AddSingleton<Services.CatalogosService>();
     
            #endregion

            return builder.Build();
        }
    }
}
