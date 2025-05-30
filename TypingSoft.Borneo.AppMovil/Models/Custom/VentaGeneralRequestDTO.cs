﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingSoft.Borneo.AppMovil.Models.API;

namespace TypingSoft.Borneo.AppMovil.Models.Custom
{
    public class VentaGeneralRequestDTO: ResponseBase
    {
        public VentaGeneralRequestDTO()
        {
            this.Data = new List<VentaGeneral>();
        }
        public List<VentaGeneral> Data { get; set; }

        public class VentaGeneral
        {
            private Guid idRuta;
            public Guid IdRuta
            {
                get => idRuta;
                set => idRuta = value;
            }

            private int vuelta;
            public int Vuelta
            {
                get => vuelta;
                set => vuelta = value;
            }
            private DateTime fecha;
            public DateTime Fecha
            {
                get => fecha;
                set => fecha = value;
            }   
        }

    }
}
