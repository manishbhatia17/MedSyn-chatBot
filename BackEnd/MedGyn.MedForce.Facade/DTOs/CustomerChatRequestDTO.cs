using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.DTOs
{
    public class CustomerChatRequestDTO : IChatWidgetRequest
    {
        public int ChatLogId { get; set; }
        public string Message { get; set; }
        public int? CustomerId { get; set; }
        public string FunctionHint { get; set; }
        public string Country { get; set; }
        public string CompanyId { get; set; }
    }
}
