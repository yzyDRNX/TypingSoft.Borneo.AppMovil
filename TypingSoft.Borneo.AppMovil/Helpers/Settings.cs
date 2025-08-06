using Microsoft.Maui.Storage;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {
        private const string UrlBaseAPIKey = "UrlBaseAPI";
        private const string DefaultUrlBaseAPI = "http://192.168.1.155:45455/API/";

        public static string? UrlBaseAPI
        {
            get => Preferences.Default.ContainsKey(UrlBaseAPIKey)
                ? Preferences.Default.Get<string?>(UrlBaseAPIKey, null)
                : null;
            set => Preferences.Default.Set(UrlBaseAPIKey, value);
        }

        public static string? UltimaDescripcionRuta { get; set; }
        public static Guid IdRuta { get; set; }
        public static Guid IdClienteAsociado { get; set; }
        public static Guid IdVentaGeneral { get; set; }
    }
}