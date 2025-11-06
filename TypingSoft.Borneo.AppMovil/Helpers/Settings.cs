using Microsoft.Maui.Storage;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class Settings
    {
        private const string UrlBaseAPIKey = "UrlBaseAPI";
        private const string DefaultUrlBaseAPI = "http://192.168.100.8/API/";

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

        // MÉTODOS UTILITARIOS PARA GUARDAR Y RECUPERAR VALORES

        public static void FijarConfiguracion(string key, object value)
        {
            if (value is string s)
                Preferences.Default.Set(key, s);
            else if (value is int i)
                Preferences.Default.Set(key, i);
            else if (value is bool b)
                Preferences.Default.Set(key, b);
            else if (value is Guid g)
                Preferences.Default.Set(key, g.ToString());
            else if (value is double d)
                Preferences.Default.Set(key, d);
            else
                Preferences.Default.Set(key, value?.ToString() ?? string.Empty);
        }

        public static T ObtenerValor<T>(string key)
        {
            if (!Preferences.Default.ContainsKey(key))
                return default!;

            if (typeof(T) == typeof(Guid))
            {
                var str = Preferences.Default.Get<string>(key, string.Empty);
                return (T)(object)(Guid.TryParse(str, out var g) ? g : Guid.Empty);
            }
            return Preferences.Default.Get<T>(key, default!);
        }
    }
}