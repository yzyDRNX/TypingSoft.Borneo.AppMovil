using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingSoft.Borneo.AppMovil.Models.API
{
   public class CustomResponse<T>: ResponseBase
    {
        public T Data { get; set; }
    }
}
