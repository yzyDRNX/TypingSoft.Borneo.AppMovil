using System.Text;
using TypingSoft.Borneo.AppMovil.Local;
using TypingSoft.Borneo.AppMovil.Services;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public static class TicketFormatter
    {
        // Ancho estándar 58mm
        private const int TicketWidth = 30;

        // Comandos ESC/POS estándar (compatibles con Bixolon)
        private const string ESC_INIT = "\x1B\x40";
        private const string ESC_ALIGN_LEFT = "\x1B\x61\x00";
        private const string ESC_ALIGN_CENTER = "\x1B\x61\x01";
        private const string ESC_ALIGN_RIGHT = "\x1B\x61\x02";
        private const string ESC_BOLD_ON = "\x1B\x45\x01";
        private const string ESC_BOLD_OFF = "\x1B\x45\x00";
        private const string GS_SIZE_NORMAL = "\x1D\x21\x00";
        private const string GS_SIZE_2X = "\x1D\x21\x11";

        private const string SEPARADOR = "------------------------------";

        public static async Task<string> FormatearTicketLocalAsync(
            LocalDatabaseService localDb,
            TicketDetalleLocal ticket,
            TicketDetalleLocal detalles,
            int numeroImpresiones,
            bool mostrarPrecio,
            bool mostrarProducto,
            int folio)
        {
            var condicion = !string.IsNullOrWhiteSpace(ticket.CondicionPago)
                ? ticket.CondicionPago
                : await localDb.ObtenerCondicionPagoTextoPorClienteAsociadoAsync(ticket.IdCliente);

            var raw = ConstruirTicket(ticket, detalles, numeroImpresiones, mostrarPrecio, mostrarProducto, condicion, folio);

            var idRuta = await localDb.ObtenerIdRutaAsync() ?? Guid.Empty;
            var barcodePayload = BuildBarcodePayload(idRuta, folio);

            var sb = new StringBuilder(raw);

            // ✅ Ajuste: impresión directa para Bixolon
            AppendBarcodeBixolon(sb, barcodePayload);

            return sb.ToString();
        }

        private static string ConstruirTicket(
            TicketDetalleLocal ticket,
            TicketDetalleLocal detalles,
            int numeroImpresiones,
            bool mostrarPrecio,
            bool mostrarProducto,
            string? condicion,
            int? folio)
        {
            const int anchoCantidad = 4;
            const int anchoDescripcion = 14;
            const int anchoImporte = 9;

            var sb = new StringBuilder();

            sb.Append(ESC_INIT);
            sb.Append(ESC_ALIGN_LEFT);
            sb.Append(GS_SIZE_NORMAL);

            string tipoCopia = numeroImpresiones <= 1 ? "ORIGINAL" : "REIMPRESION";

            sb.AppendLine(SEPARADOR);

            if (!string.IsNullOrWhiteSpace(condicion))
            {
                sb.Append(ESC_ALIGN_CENTER);
                sb.Append(ESC_BOLD_ON);
                sb.Append(GS_SIZE_2X);
                string condicionTexto = condicion.Length > 15 ? condicion.Substring(0, 15) : condicion;
                sb.AppendLine(condicionTexto.ToUpperInvariant());
                sb.Append(GS_SIZE_NORMAL);
                sb.Append(ESC_BOLD_OFF);
                sb.Append(ESC_ALIGN_LEFT);
            }
            else
            {
                sb.AppendLine();
            }

            sb.AppendLine(SEPARADOR);

            sb.Append(ESC_ALIGN_CENTER);
            sb.AppendLine("Agua Purificada Borneo");
            sb.AppendLine("Tuxtla Gutierrez, Chiapas");
            sb.AppendLine("RFC: APB080318M65");
            sb.AppendLine("Tel: 961 614 05 47");
            sb.Append(ESC_ALIGN_LEFT);

            sb.AppendLine(SEPARADOR);

            sb.Append(ESC_ALIGN_CENTER);
            sb.Append(ESC_BOLD_ON);
            sb.Append(GS_SIZE_2X);
            string tipoCopiaTexto = tipoCopia.Length > 15 ? tipoCopia.Substring(0, 15) : tipoCopia;
            sb.AppendLine(tipoCopiaTexto);
            sb.Append(GS_SIZE_NORMAL);
            sb.Append(ESC_BOLD_OFF);
            sb.Append(ESC_ALIGN_LEFT);

            sb.AppendLine(SEPARADOR);

            if (folio.HasValue)
            {
                sb.AppendLine(TruncateToWidth($"FOLIO: {folio.Value:D6}", TicketWidth));
                sb.AppendLine(SEPARADOR);
            }

            string clienteLinea = $"CLIENTE: {ticket.Cliente}";
            if (clienteLinea.Length > TicketWidth)
            {
                sb.AppendLine("CLIENTE:");
                sb.AppendLine(TruncateToWidth(ticket.Cliente, TicketWidth));
            }
            else
            {
                sb.AppendLine(TruncateToWidth(clienteLinea, TicketWidth));
            }
            sb.AppendLine(SEPARADOR);

            string empleadoLinea = $"ATENDIO: {ticket.Empleado}";
            if (empleadoLinea.Length > TicketWidth)
            {
                sb.AppendLine("ATENDIO:");
                sb.AppendLine(TruncateToWidth(ticket.Empleado, TicketWidth));
            }
            else
            {
                sb.AppendLine(TruncateToWidth(empleadoLinea, TicketWidth));
            }
            sb.AppendLine(SEPARADOR);

            if (mostrarPrecio)
                sb.AppendLine("CANT DESCRIPCION   IMPORTE");
            else
                sb.AppendLine("CANT DESCRIPCION");

            sb.AppendLine(SEPARADOR);

            decimal total = 0;
            //foreach (var d in detalles)
            //{
                string cantidad = detalles.Cantidad.ToString().PadRight(anchoCantidad);
                if (mostrarPrecio)
                {
                    string descripcion = TruncateToWidth(detalles.Descripcion ?? "", anchoDescripcion)
                        .PadRight(anchoDescripcion);
                    string importe = detalles.ImporteTotal.ToString("N2", CultureInfo.InvariantCulture)
                        .PadLeft(anchoImporte);
                    sb.AppendLine($"{cantidad} {descripcion} {importe}");
                    total += detalles.ImporteTotal;
                }
                else
                {
                    string descripcion = TruncateToWidth(detalles.Descripcion ?? "", TicketWidth - anchoCantidad - 1);
                    sb.AppendLine($"{cantidad} {descripcion}");
                }
            //}

            sb.AppendLine(SEPARADOR);

            if (mostrarPrecio)
            {
                string totalLinea = $"TOTAL: ${total:N2}";
                sb.Append(ESC_ALIGN_RIGHT);
                sb.AppendLine(TruncateToWidth(totalLinea, TicketWidth));
                sb.Append(ESC_ALIGN_LEFT);
            }

            sb.AppendLine("+---(NOMBRE Y FIRMA)-------+");
            sb.AppendLine("|                          |");
            sb.AppendLine("|                          |");
            sb.AppendLine("|                          |");
            sb.AppendLine("+--------------------------+");
            sb.AppendLine();

            sb.Append(ESC_ALIGN_CENTER);
            sb.AppendLine("RECIBI GARRAFONES");
            sb.AppendLine("COMPLETOS Y EN BUEN ESTADO");
            sb.Append(ESC_ALIGN_LEFT);

            var fechaTexto = ticket.Fecha.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            sb.AppendLine(TruncateToWidth($"FECHA: {fechaTexto}", TicketWidth));
            sb.AppendLine(SEPARADOR);

            return sb.ToString();
        }

        private static string BuildBarcodePayload(Guid idRuta, int folio)
        {
            var ruta6 = idRuta == Guid.Empty ? "000000" : idRuta.ToString("N")[..6].ToUpperInvariant();
            return $"R{ruta6}F{folio:D6}";
        }

        // ✅ Código corregido para Bixolon
        private static void AppendBarcodeBixolon(StringBuilder sb, string data)
        {
            sb.AppendLine();
            sb.Append(ESC_ALIGN_CENTER);

            // Configura el código de barras
            sb.Append("\x1D\x48\x02"); // HRI debajo
            sb.Append("\x1D\x77\x02"); // Ancho módulo
            sb.Append("\x1D\x68\x50"); // Altura

            // CODE128
            sb.Append("\x1D\x6B\x49");
            sb.Append((char)data.Length);
            sb.Append(data);
            sb.Append("\x00"); // Final obligatorio para Bixolon

            // 🔹 Avanza algunas líneas y descarga el buffer
            sb.Append("\x0A\x0A\x0A");
            sb.Append(ESC_ALIGN_LEFT);

            // Texto final
            sb.AppendLine(SEPARADOR);
            sb.AppendLine("GRACIAS POR SU PREFERENCIA");
            sb.AppendLine(SEPARADOR);

            // Avanza 5 líneas y corta (flush)
            sb.Append("\x1B\x64\x05");
            sb.Append("\x1D\x56\x00");
        }

        private static string TruncateToWidth(string text, int width)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Trim();
            return text.Length > width ? text.Substring(0, width) : text;
        }
    }
}
