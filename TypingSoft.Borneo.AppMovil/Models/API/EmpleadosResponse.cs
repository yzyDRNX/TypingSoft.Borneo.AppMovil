namespace TypingSoft.Borneo.AppMovil.Models.API
{
    public class EmpleadosResponse : ResponseBase
    {
        public EmpleadosResponse()
        {
            this.Data = new List<Empleados>();
        }
        public List<Empleados> Data { get; set; }

        public class Empleados
        {
            public Guid Id { get; set; }
            public string? Empleado { get; set; }
        }
    }
}
