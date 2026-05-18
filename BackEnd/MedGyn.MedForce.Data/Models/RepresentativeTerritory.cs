using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class RepresentativeTerritory
    {
        public virtual int Id { get; set; }
        public virtual Representative Representative { get; set; }

        public virtual Territory Territory { get; set; }
    }
}
