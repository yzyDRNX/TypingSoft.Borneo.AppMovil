using System.Text;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        // Llama a este método pasando el ticket y el número de impresiones previas
        public static string FormatearTicketLocal(TicketLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones)
        {
            var sb = new StringBuilder();
            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine("--------------------------------");
            sb.AppendLine("            BORNEO              ");
            sb.AppendLine("    Agua Purificada Borneo");
            sb.AppendLine("          S.A. de C.V.");
            sb.AppendLine("   Tuxtla Gutierrez, Chiapas    ");
            sb.AppendLine("          APB080318M65          ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            sb.AppendLine($"         *** {tipoCopia} ***         ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"CLIENTE: {ticket.Cliente}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("CANT  DESCRIPCION       IMPORTE");
            sb.AppendLine("--------------------------------");

            decimal total = 0;
            foreach (var d in detalles)
            {
                string cantidad = d.Cantidad.ToString().PadRight(4);
                string descripcion = (d.Descripcion ?? "").Length > 17
                    ? d.Descripcion.Substring(0, 17)
                    : (d.Descripcion ?? "").PadRight(17);
                string importe = d.Importe.ToString("N2").PadLeft(8);

                sb.AppendLine($"{cantidad} {descripcion} {importe}");
                total += d.Importe;
            }

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL:                ${total:N2}".PadLeft(31));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("         NOMBRE Y FIRMA:");
            sb.AppendLine();
            sb.AppendLine($"ATENDIO: {ticket.Empleado}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("AL PONER SU FIRMA ESTA DE ACUERDO CON LA INFORMACION");
            sb.AppendLine("QUE CONTIENE LA NOTA. CUALQUIER ANOMALIA EN ESTA NOTA");
            sb.AppendLine("DE REMISION REPORTELA AL TELEFONO: 6140 547");
            sb.AppendLine();
            sb.AppendLine("   ***GRACIAS POR SU COMPRA***  ");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"FECHA: {ticket.Fecha:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("      [ AREA PARA LOGO / INFO ] ");
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }
    }
}
