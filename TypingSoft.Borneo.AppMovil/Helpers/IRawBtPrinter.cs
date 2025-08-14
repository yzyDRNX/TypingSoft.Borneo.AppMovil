public interface IRawBtPrinter
{
    Task PrintTextAsync(string text);
    Task PrintBytesAsync(byte[] data);
}