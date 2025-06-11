namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {
#if ANDROID
        public const string UrlBaseAPI = "http://192.168.100.7:45455/API/";
#else
        public const string UrlBaseAPI = "http://192.168.100.7:45455/api/";
#endif

        public static string? UltimaDescripcionRuta { get; set; }
        public static Guid IdRuta { get; set; }
        public static Guid IdClienteAsociado { get; set; }
    }
}