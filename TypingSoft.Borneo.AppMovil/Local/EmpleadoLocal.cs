﻿using SQLite;

namespace TypingSoft.Borneo.AppMovil.Local
{
    [Table("Empleados")]
    public class EmpleadoLocal
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("Id")]
        public Guid Id { get; set; }
        [Column("Empleado")]
        public string? Empleado { get; set; }
        
    }
}
