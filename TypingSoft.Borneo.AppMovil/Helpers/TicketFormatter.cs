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
            sb.AppendLine();

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

            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("            BORNEO              ");
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
            sb.AppendLine();

            // NUEVO: Folio antes del nombre del cliente
            if (folio.HasValue)
            {
                // Formato D6 (relleno con ceros). Ajusta si quieres más/menos dígitos.
                sb.AppendLine($" FOLIO: {folio.Value:D6}");
            }

            sb.AppendLine($" {ticket.Cliente}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            // Encabezado dinámico
            if (mostrarPrecio && mostrarProducto)
                sb.AppendLine("CANT  PRODUCTO       IMPORTE");
            else if (mostrarPrecio && !mostrarProducto)
                sb.AppendLine("CANT                IMPORTE");
            else if (!mostrarPrecio && mostrarProducto)
                sb.AppendLine("CANT  PRODUCTO");
            else
                sb.AppendLine("CANT");

            sb.AppendLine();
            sb.AppendLine("--------------------------------");

            decimal total = 0;
            foreach (var d in detalles)
            {
                string cantidad = d.Cantidad.ToString().PadRight(anchoCantidad);
                string descripcionReal = d.Descripcion ?? string.Empty;
                if (descripcionReal.Length > anchoDescripcion)
                    descripcionReal = descripcionReal[..anchoDescripcion];
                else
                    descripcionReal = descripcionReal.PadRight(anchoDescripcion);

                string descripcionCol = mostrarProducto ? descripcionReal : new string(' ', anchoDescripcion);

                if (mostrarPrecio)
                {
                    string importe = d.ImporteTotal.ToString("N2").PadLeft(anchoImporte);
                    if (mostrarProducto)
                        sb.AppendLine($"{cantidad} {descripcionCol} {importe}");
                    else
                        sb.AppendLine($"{cantidad} {descripcionCol} {importe}".TrimEnd());
                    total += d.ImporteTotal;
                }
                else
                {
                    if (mostrarProducto)
                        sb.AppendLine($"{cantidad} {descripcionCol}");
                    else
                        sb.AppendLine($"{cantidad}");
                }
            }

            sb.AppendLine("--------------------------------");
            if (mostrarPrecio)
                sb.AppendLine($"TOTAL:                ${total:N2}".PadLeft(31));
            sb.AppendLine("+-------(NOMBRE Y FIRMA)-------+");
            for (int i = 0; i < 11; i++)
                sb.AppendLine("|                              |");
            sb.AppendLine("+------------------------------+");
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

            var fechaTexto = ticket.Fecha.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            sb.AppendLine($"FECHA: {fechaTexto}");
            sb.AppendLine();
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

            sb.AppendLine();
            sb.AppendLine("--------------------------------");
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