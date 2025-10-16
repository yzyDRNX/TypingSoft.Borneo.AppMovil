#if ANDROID
using Android.Bluetooth;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Android;
using Android.Content.PM;
using Android.App;
using System.Collections.Generic;
using JIOException = Java.IO.IOException;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class RawBtPrinter : IRawBtPrinter
    {
        private string _printerName;

        public RawBtPrinter(string printerName)
        {
            _printerName = printerName;
        }

        public void SetPrinterName(string printerName)
        {
            _printerName = printerName;
        }

        public async Task PrintTextAsync(string text)
        {
            await PrintBytesAsync(Encoding.UTF8.GetBytes(text));
        }

        public async Task PrintBytesAsync(byte[] data)
        {
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("Bluetooth no está disponible en este dispositivo.");
            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth está deshabilitado. Actívalo e inténtalo de nuevo.");

            // Coincidencia exacta (ignorando mayúsculas/minúsculas) para evitar falsas coincidencias
            var device = adapter.BondedDevices?
                .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.Name) &&
                                     d.Name.Equals(_printerName, StringComparison.OrdinalIgnoreCase));

            if (device == null)
                throw new Exception($"Impresora '{(_printerName ?? "<sin nombre>")}' no encontrada o no emparejada. Empareja primero la impresora en ajustes de Bluetooth.");

            // Evita que la búsqueda/discovery interfiera con la conexión RFCOMM
            adapter.CancelDiscovery();

            var uuid = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            using var socket = device.CreateRfcommSocketToServiceRecord(uuid);
            try
            {
                await socket.ConnectAsync().ConfigureAwait(false);
                await socket.OutputStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                await socket.OutputStream.FlushAsync().ConfigureAwait(false);
            }
            catch (JIOException ioEx)
            {
                // Fallo típico cuando la impresora está apagada, fuera de alcance o el servicio no responde
                throw new Exception("No se pudo conectar o enviar datos a la impresora. Verifica que esté encendida, dentro del alcance y emparejada.", ioEx);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error inesperado al imprimir por Bluetooth.", ex);
            }
            finally
            {
                try { socket.Close(); } catch { /* Ignorar */ }
            }
        }

        // Método para obtener la lista de dispositivos emparejados
        public static List<string> GetBondedPrinterNames()
        {
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                return new List<string>();

            return adapter.BondedDevices
                .Select(d => d.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .OrderBy(n => n)
                .ToList();
        }
    }
}
#else
namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class RawBtPrinter : IRawBtPrinter
    {
        public RawBtPrinter(string printerName) { }
        public void SetPrinterName(string printerName) { }
        public Task PrintTextAsync(string text) => Task.CompletedTask;
        public Task PrintBytesAsync(byte[] data) => Task.CompletedTask;
        public static List<string> GetBondedPrinterNames() => new List<string>();
    }
}
#endif