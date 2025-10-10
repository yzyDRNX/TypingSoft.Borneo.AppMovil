using System.Text;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;
using System.Threading.Tasks;
using System.Globalization;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        private const int TicketWidth = 32;

        private const string ESC_ALIGN_LEFT = "\x1B\x61\x00";
        private const string ESC_ALIGN_CENTER = "\x1B\x61\x01";
        private const string ESC_BOLD_ON = "\x1B\x45\x01";
        private const string ESC_BOLD_OFF = "\x1B\x45\x00";
        private const string GS_SIZE_NORMAL = "\x1D\x21\x00";
        private const string GS_SIZE_2X = "\x1D\x21\x11";



        // Nuevo con mostrarProducto y folio visible
        public static async Task<string> FormatearTicketLocalAsync(
            LocalDatabaseService localDb,
            TicketDetalleLocal ticket,
            List<TicketDetalleLocal> detalles,
            int numeroImpresiones,
            bool mostrarPrecio,
            bool mostrarProducto,
            int folio)
        {
            var condicion = !string.IsNullOrWhiteSpace(ticket.CondicionPago)
                ? ticket.CondicionPago
                : await localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(ticket.IdCliente);

            // Ahora se pasa el folio para que se muestre antes del cliente
            var raw = ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, mostrarProducto, condicion, folio);

            var idRuta = await localDb.ObtenerIdRutaAsync() ?? Guid.Empty;
            var barcodePayload = BuildBarcodePayload(idRuta, folio);

            var sb = new StringBuilder(raw);
            AppendBarcodeCode128(sb, barcodePayload);

            return sb.ToString();
        }

        private static string ConstruirTicket(
            TicketDetalleLocal ticket,
            List<TicketDetalleLocal> detalles,
            int numeroImpresiones,
            bool mostrarPrecio,
            bool mostrarProducto,
            string? condicion,
            int? folio)
        {
            const int anchoCantidad = 4;
            const int anchoDescripcion = 17;
            const int anchoImporte = 8;

            var sb = new StringBuilder();
            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine("--------------------------------");

            if (!string.IsNullOrWhiteSpace(condicion))
            {
                sb.Append(ESC_ALIGN_CENTER);
                sb.Append(ESC_BOLD_ON);
                sb.Append(GS_SIZE_2X);
                sb.AppendLine(condicion.ToUpperInvariant());
                sb.Append(GS_SIZE_NORMAL);
                sb.Append(ESC_BOLD_OFF);
                sb.Append(ESC_ALIGN_LEFT);
            }
            else
            {
                sb.AppendLine();
            }
            sb.AppendLine("--------------------------------");
            sb.AppendLine("    Agua Purificada Borneo");
            sb.AppendLine("   Tuxtla Gutierrez, Chiapas    ");
            sb.AppendLine("      RFC: APB080318M65         ");
            sb.AppendLine("    Telefono: 961 614 05 47     ");
            sb.AppendLine("--------------------------------");

            sb.Append(ESC_ALIGN_CENTER);
            sb.Append(ESC_BOLD_ON);
            sb.Append(GS_SIZE_2X);
            sb.AppendLine($"   {tipoCopia}   ");
            sb.Append(GS_SIZE_NORMAL);
            sb.Append(ESC_BOLD_OFF);
            sb.Append(ESC_ALIGN_LEFT);

            sb.AppendLine("--------------------------------");

            // NUEVO: Folio antes del nombre del cliente
            if (folio.HasValue)
            {
                sb.AppendLine($" FOLIO: {folio.Value:D6}");
            }
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"CLIENTE: {ticket.Cliente}");
            sb.AppendLine("--------------------------------");
            sb.AppendLine($"ATENDIO: {ticket.Empleado}");
            sb.AppendLine("--------------------------------");
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
            sb.AppendLine("+-------(NOMBRE Y FIRMA)-------+");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("+------------------------------+");
            sb.AppendLine();
            sb.AppendLine("   RECIBI GARRAFONES ");
            sb.AppendLine("   COMPLETOS Y EN BUEN ESTADO");
            var fechaTexto = ticket.Fecha.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            sb.AppendLine($"FECHA: {fechaTexto}");
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }

        private static string BuildBarcodePayload(Guid idRuta, int folio)
        {
            var ruta6 = idRuta == Guid.Empty ? "000000" : idRuta.ToString("N")[..6].ToUpperInvariant();
            return $"R{ruta6}F{folio:D6}";
        }

        private static void AppendBarcodeCode128(StringBuilder sb, string data)
        {
            sb.Append(ESC_ALIGN_CENTER);
            sb.Append(ESC_BOLD_ON);
            sb.Append(ESC_BOLD_OFF);

            sb.Append("\x1D\x48\x02");
            sb.Append("\x1D\x66\x00");
            sb.Append("\x1D\x77\x01");
            sb.Append("\x1D\x68\x50");

            var payload = "{B" + data;
            if (payload.Length > 255)
                payload = payload[..255];

            sb.Append("\x1D\x6B\x49");
            sb.Append((char)payload.Length);
            sb.Append(payload);

            sb.AppendLine("--------------------------------");
            sb.AppendLine("***GRACIAS POR SU PREFERENCIA***");
            sb.Append(ESC_ALIGN_LEFT);
        }

        private static string CenterText(string text, int width)
        {
            text = (text ?? string.Empty).Trim();
            if (text.Length > width)
                text = text[..width];

            int left = (width - text.Length) / 2;
            int right = width - text.Length - left;

            return new string(' ', left) + text + new string(' ', right);
        }
    }
}