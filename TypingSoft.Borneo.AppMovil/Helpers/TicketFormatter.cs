using System.Text;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        public static string FormatearTicket(VentaGeneralResponse venta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("TICKET DE VENTA");
            sb.AppendLine("----------------------");
            // Aquí puedes agregar más datos según tu modelo
            if (venta?.Data != null)
            {
                foreach (var item in venta.Data)
                {
                    sb.AppendLine($"Producto: {item.IdRuta}  Cantidad: {item.Vuelta}  Total: {item.Fecha}");
                }
            }
            sb.AppendLine("----------------------");
            // Puedes agregar totales, fecha, cliente, etc.
            return sb.ToString();
        }
    }
}
