namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {

        public const string UrlBaseAPI = "http://192.168.1.250:45455/API/";

        public static string? UltimaDescripcionRuta { get; set; }
        public static Guid IdRuta { get; set; }
        public static Guid IdClienteAsociado { get; set; }

        public static Guid IdVentaGeneral { get; set; }
    }
}