namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {
#if ANDROID
        public const string UrlBaseAPI = "http://10.0.2.2:5099/api/";
#else
         public const string UrlBaseAPI = "http://192.168.1.242:5099/api/";
#endif

        public static string? UltimaDescripcionRuta { get; set; }
        public static Guid IdRuta { get; set; }
        public static Guid IdClienteAsociado { get; set; }
    }
}
