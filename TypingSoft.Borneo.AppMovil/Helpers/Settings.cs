namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {
#if ANDROID
        public const string UrlBaseAPI = "https://192.168.1.242:45455/api/";
#else
        public const string UrlBaseAPI = "https://192.168.1.242:45455/api/";
#endif

        public static string? UltimaDescripcionRuta { get; set; }
        public static Guid IdRuta { get; set; }
        public static Guid IdClienteAsociado { get; set; }
    }
}