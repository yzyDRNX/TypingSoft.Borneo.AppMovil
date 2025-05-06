namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class RutaResponse : ResponseBase
    {
        public RutaResponse()
        {
            this.Data = new Rutas();
        }

        public Rutas Data { get; set; }

        public class Rutas
        {
            public Guid Id { get; set; }
            public string? Descripcion { get; set; }
        }
    }
}
