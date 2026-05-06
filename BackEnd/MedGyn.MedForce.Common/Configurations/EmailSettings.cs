using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.Configurations
{
    public class EmailSettings
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromDisplayName { get; set; }
    }
}
