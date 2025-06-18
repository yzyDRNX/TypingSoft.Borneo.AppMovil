using System.Text;
using TypingSoft.Borneo.AppMovil.Models.API;
using TypingSoft.Borneo.AppMovil.Local;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        
        // Llama a este método pasando el ticket y el número de impresiones previas
        public static string FormatearTicketLocal(TicketLocal ticket, int numeroImpresiones)
        {
            var sb = new StringBuilder();

            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine("--------------------------------");
            sb.AppendLine("            BORNEO       ");
            sb.AppendLine("  Calle Ejemplo #123, Centro    ");
            sb.AppendLine(" Agua Purificada Borneo SA de CV    ");
            sb.AppendLine("   Tuxtla Gutierrez, Chiapas    ");
            sb.AppendLine("         APB080318M65    ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            sb.AppendLine($"{tipoCopia.PadRight(31)}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"CLIENTE: {ticket.Cliente}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine("CANT DESC.        IMPORTE");
            sb.AppendLine("--------------------------------");

            // Solo un producto por ticket (según tu modelo actual)
            string formattedQuantity = ticket.Cantidad.ToString().PadRight(4);
            string formattedDescription = (ticket.Descripcion ?? "").Length > 17
                ? ticket.Descripcion.Substring(0, 17)
                : (ticket.Descripcion ?? "").PadRight(17);
            string formattedAmount = ticket.ImporteTotal.ToString("N2").PadLeft(8);

            sb.AppendLine($"{formattedQuantity}{formattedDescription}{formattedAmount}");

            sb.AppendLine("--------------------------------");
            sb.AppendLine($"TOTAL:             ${ticket.ImporteTotal:N2}".PadLeft(31));
            sb.AppendLine();
            sb.AppendLine("NOMBRE Y FIRMA:");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"ATENDIO: {ticket.Empleado}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("AL PONER SU FIRMA ESTA DE ACUERDO CON LA INFORMACION");
            sb.AppendLine("QUE CONTIENE LA NOTA, CUALQUIER ANOMALIA EN ESTA NOTA");
            sb.AppendLine("DE REMISION REPORTARLO AL TELEFONO: 6140 547");
            sb.AppendLine();
            sb.AppendLine("     GRACIAS POR SU COMPRA !    ");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"FECHA: {ticket.Fecha:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("      [AREA PARA LOGO/INFO]     ");
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }
    }
}


