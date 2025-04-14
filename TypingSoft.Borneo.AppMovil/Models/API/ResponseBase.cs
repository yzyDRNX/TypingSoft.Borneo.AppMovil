namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class ResponseBase
    {
        public ResponseBase()
        {
            this.Mensaje = string.Empty;
        }
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
 
    }
}
