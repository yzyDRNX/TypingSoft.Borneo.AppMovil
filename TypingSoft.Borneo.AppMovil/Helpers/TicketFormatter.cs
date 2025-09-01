using System.Text;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        // Método existente (sincrónico) - deja el placeholder
        public static string FormatearTicketLocal(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio)
        {
            return ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, "Contado o Credito");
        }

        // NUEVO: usa IdCliente del ticket (que en tu flujo guarda IdClienteAsociado) para resolver la condición
        public static async Task<string> FormatearTicketLocalAsync(LocalDatabaseService localDb, TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio)
        {
            // Importante: en tu flujo, TicketDetalleLocal.IdCliente contiene el IdClienteAsociado
            var condicionTexto = await localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(ticket.IdCliente);
            return ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, $"CONDICION: {condicionTexto}");
        }

        private static string ConstruirTicket(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio, string encabezadoCondicion)
        {
            var sb = new StringBuilder();
            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine("--------------------------------");
            sb.AppendLine();
            sb.AppendLine(encabezadoCondicion);
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("            BORNEO              ");
            sb.AppendLine("    Agua Purificada Borneo");
            sb.AppendLine("   Tuxtla Gutierrez, Chiapas    ");
            sb.AppendLine("      RFC: APB080318M65         ");
            sb.AppendLine("    Telefono: 961 614 05 47     ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"         *** {tipoCopia} ***         ");
            sb.AppendLine("--------------------------------");
            sb.AppendLine();
            sb.AppendLine($"CLIENTE: {ticket.Cliente}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine();
            sb.AppendLine(mostrarPrecio ? "CANT  DESCRIPCION       IMPORTE" : "CANT  DESCRIPCION");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");

            decimal total = 0;
            foreach (var d in detalles)
            {
                string cantidad = d.Cantidad.ToString().PadRight(4);
                string descripcion = (d.Descripcion ?? "").Length > 17
                    ? d.Descripcion.Substring(0, 17)
                    : (d.Descripcion ?? "").PadRight(17);

                if (mostrarPrecio)
                {
                    string importe = d.ImporteTotal.ToString("N2").PadLeft(8);
                    sb.AppendLine($"{cantidad} {descripcion} {importe}");
                    total += d.ImporteTotal;
                }
                else
                {
                    sb.AppendLine($"{cantidad} {descripcion}");
                }
            }

            sb.AppendLine("--------------------------------");
            if (mostrarPrecio)
                sb.AppendLine($"TOTAL:                ${total:N2}".PadLeft(31));
            sb.AppendLine("+---------------(FIRMA )------------+");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("|                                   |");
            sb.AppendLine("+-----------------------------------+");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"ATENDIO: {ticket.Empleado}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("   AL PONER SU FIRMA ESTA DE ");
            sb.AppendLine("   ACUERDO CON LA INFORMACION");
            sb.AppendLine("   QUE CONTIENE LA NOTA.");
            sb.AppendLine();
            sb.AppendLine("   ***GRACIAS POR SU COMPRA***  ");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"FECHA: {ticket.Fecha:dd/MM/yyyy}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }
    }
}
