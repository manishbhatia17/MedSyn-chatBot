using MedGyn.MedForce.Facade.Handlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.DTOs
{
    public class CustomerChatResponseDTO
    {
        public string FunctionName { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
    }
}
