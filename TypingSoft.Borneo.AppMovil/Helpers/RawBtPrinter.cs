#if ANDROID
using Android.Bluetooth;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Android;
using Android.Content.PM;
using Android.App;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class RawBtPrinter : IRawBtPrinter
    {
        public async Task PrintTextAsync(string text)
        {
            await PrintBytesAsync(Encoding.UTF8.GetBytes(text));
        }

        public async Task PrintBytesAsync(byte[] data)
        {
            // Permisos y conexión igual que antes...
            const string printerName = "PT-210_CE60";
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null || !adapter.IsEnabled)
                throw new Exception("Bluetooth no está disponible o habilitado.");

            var device = adapter.BondedDevices.FirstOrDefault(d => d.Name.Contains(printerName));
            if (device == null)
                throw new Exception("Impresora no encontrada. Empareja primero la impresora en ajustes de Bluetooth.");

            var uuid = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            using var socket = device.CreateRfcommSocketToServiceRecord(uuid);
            await socket.ConnectAsync();

            await socket.OutputStream.WriteAsync(data, 0, data.Length);

            socket.Close();
        }
    }
}
#endif