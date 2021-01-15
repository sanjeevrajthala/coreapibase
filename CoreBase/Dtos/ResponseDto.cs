using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Dtos
{
    public class ResponseDto
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}
