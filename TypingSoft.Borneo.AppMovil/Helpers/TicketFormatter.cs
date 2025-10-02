using System.Text;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;
using System.Threading.Tasks;
using System.Globalization;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        // Ancho de línea del ticket (coincide con "--------------------------------")
        private const int TicketWidth = 32;

        // Comandos ESC/POS básicos para cambiar tamaño/alineación/negrita
        private const string ESC_ALIGN_LEFT = "\x1B\x61\x00";
        private const string ESC_ALIGN_CENTER = "\x1B\x61\x01";
        private const string ESC_BOLD_ON = "\x1B\x45\x01";
        private const string ESC_BOLD_OFF = "\x1B\x45\x00";
        private const string GS_SIZE_NORMAL = "\x1D\x21\x00"; // Tamaño normal
        private const string GS_SIZE_2X = "\x1D\x21\x11";     // Doble ancho y doble alto

        [System.Obsolete("Usa FormatearTicketLocalAsync(LocalDatabaseService, ... ) para imprimir la condición real por cliente.")]
        public static string FormatearTicketLocal(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio)
        {
            // Sin BD, no imprimimos condición
            return ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, null);
        }

        // RECOMENDADO
        public static async Task<string> FormatearTicketLocalAsync(LocalDatabaseService localDb, TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio)
        {
            // Preferir snapshot guardado en el ticket; si no, consultar BD
            var condicion = !string.IsNullOrWhiteSpace(ticket.CondicionPago)
                ? ticket.CondicionPago
                : await localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(ticket.IdCliente);

            // Construye el texto base del ticket
            var raw = ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, condicion);

            // Obtener IdRuta y último folio local (ajusta si ya tienes el folio del ticket)
            var idRuta = await localDb.ObtenerIdRutaAsync() ?? Guid.Empty;
            var folio = await localDb.ObtenerUltimoValorFolioVentaAsync(); // usa tu folio actual aquí si lo tienes

            // Armar payload compacto para el código de barras
            var barcodePayload = BuildBarcodePayload(idRuta, folio);

            // Anexar el código de barras ESC/POS (CODE128)
            var sb = new StringBuilder(raw);
            AppendBarcodeCode128(sb, barcodePayload);

            return sb.ToString();
        }

        private static string ConstruirTicket(TicketDetalleLocal ticket, List<TicketDetalleLocal> detalles, int numeroImpresiones, bool mostrarPrecio, string? condicion)
        {
            var sb = new StringBuilder();
            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine("--------------------------------");
            sb.AppendLine();

            // Hacer grande la condición
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
                sb.AppendLine(); // mantiene el alto del encabezado
            }

            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.AppendLine("            BORNEO              ");
            sb.AppendLine("    Agua Purificada Borneo");
            sb.AppendLine("   Tuxtla Gutierrez, Chiapas    ");
            sb.AppendLine("      RFC: APB080318M65         ");
            sb.AppendLine("    Telefono: 961 614 05 47     ");
            sb.AppendLine("--------------------------------");

            // Hacer grande *** ORIGINAL/REIMPRESION ***
            sb.Append(ESC_ALIGN_CENTER);
            sb.Append(ESC_BOLD_ON);
            sb.Append(GS_SIZE_2X);
            sb.AppendLine($"   {tipoCopia}   ");
            sb.Append(GS_SIZE_NORMAL);
            sb.Append(ESC_BOLD_OFF);
            sb.Append(ESC_ALIGN_LEFT);

            sb.AppendLine("--------------------------------");
            sb.AppendLine();
            sb.AppendLine($" {ticket.Cliente}");
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
            sb.AppendLine("+-------(NOMBRE Y FIRMA)-------+");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
            sb.AppendLine("|                              |");
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

            // Formato 24h + cultura invariante (evita 'a. m.'/'p. m.' y NBSP)
            var fechaTexto = ticket.Fecha.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            sb.AppendLine($"FECHA: {fechaTexto}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------");

            return sb.ToString();
        }

        // Construye un payload corto: R{8hex IdRuta}F{folio 6 dígitos}, ej: R12AB34C5F000123
        private static string BuildBarcodePayload(Guid idRuta, int folio)
        {
            var ruta8 = idRuta == Guid.Empty ? "00000000" : idRuta.ToString("N").Substring(0, 8).ToUpperInvariant();
            return $"R{ruta8}F{folio:D6}";
        }

        // Agrega a sb el bloque ESC/POS para imprimir un CODE128 con HRI
        private static void AppendBarcodeCode128(StringBuilder sb, string data)
        {
            // Título (opcional)
            sb.Append(ESC_ALIGN_CENTER);
            sb.Append(ESC_BOLD_ON);
            sb.AppendLine("ESCANEA PARA VINCULAR");
            sb.Append(ESC_BOLD_OFF);

            // HRI (texto legible) debajo, fuente A
            sb.Append("\x1D\x48\x02"); // GS H n (2 = debajo)
            sb.Append("\x1D\x66\x00"); // GS f n (0 = fuente A)

            // Ancho y alto del código de barras
            sb.Append("\x1D\x77\x03"); // GS w n (2..6) - módulo
            sb.Append("\x1D\x68\x50"); // GS h n (altura, ~80 dots)

            // CODE128 (m=73) con longitud 'n'. Prefijo {B para subset B
            var payload = "{B" + data;
            if (payload.Length > 255)
                payload = payload.Substring(0, 255);

            sb.Append("\x1D\x6B\x49");        // GS k m (m=73 = CODE128)
            sb.Append((char)payload.Length);  // n = longitud
            sb.Append(payload);               // datos

            sb.AppendLine();
            sb.AppendLine("--------------------------------");
            sb.Append(ESC_ALIGN_LEFT);
        }

        private static string CenterText(string text, int width)
        {
            text = (text ?? string.Empty).Trim();
            if (text.Length > width)
                text = text.Substring(0, width);

            int left = (width - text.Length) / 2;
            int right = width - text.Length - left;

            return new string(' ', left) + text + new string(' ', right);
        }
    }
}