using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class StaticSettings
    {
        public const string IdRuta = "IdRuta";
        public const string IdCliente = "IdCliente";
        public const string Cliente = "Cliente";

        public const string IdVentaGeneral = "IdVentaGeneral";
        public const string IdClienteAsociado = "IdClienteAsociado";
        public const string IdCondicionPago = "IdCondicionPago";
        public const string IdFormaPago = "IdFormaPago";
        public const string IdProducto = "IdProducto";
        public const string Cantidad = "Cantidad";
        public const string ImporteTotal = "ImporteTotal";

        public static void FijarConfiguracion(string configuracion, string valor)
        {
            Preferences.Remove(configuracion);
            Preferences.Set(configuracion, valor);
        }
        public static void LimpiarSesion()
        {
            Preferences.Clear();
        }
        #region Obtencion de valores
        public static T ObtenerValor<T>(string configuracion)
        {
            var resultado = default(T);
            try
            {
                if (Preferences.Default.ContainsKey(configuracion))
                {
                    resultado = Preferences.Default.Get(configuracion, default(T));
                }
            }
            catch (Exception)
            {
                resultado = default(T);
            }
            return resultado;
        }
        public static Guid ObtenerValor(string configuracion)
        {
            var resultado = Guid.Empty;
            try
            {
                if (Preferences.Default.ContainsKey(configuracion))
                {
                    var preGuid = Preferences.Default.Get(configuracion, resultado.ToString());
                    resultado = Guid.Parse(preGuid);
                }
            }
            catch (Exception)
            {
                resultado = Guid.Empty;
            }
            return resultado;
        }

        public static T ObtenerValor<T>(string configuracion, T valorPredeterminado)
        {
            var resultado = valorPredeterminado;
            try
            {
                if (Preferences.Default.ContainsKey(configuracion))
                {
                    resultado = Preferences.Default.Get<T>(configuracion, default(T));
                }
            }
            catch (Exception ex)
            {
                resultado = valorPredeterminado;
            }
            return resultado;
        }
        #endregion        
    }
}
