using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class DBVersion
    {
        public int DBVersionId { get; set; }
        public string Version { get; set; }
        public DateTime RunDate { get; set; }
    }
}
