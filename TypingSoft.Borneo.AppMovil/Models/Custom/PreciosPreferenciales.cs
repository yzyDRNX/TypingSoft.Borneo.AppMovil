namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class PreciosPreferencialesLista
    {
        public PreciosPreferencialesLista()
        {

        }
        public Guid IdProducto { get; set; }
        public string? Producto { get; set; }
        public decimal Precio { get; set; }
        public Guid IdClienteAsociado { get; set; } // Identificador del cliente asociado al precio preferencial
    }
}