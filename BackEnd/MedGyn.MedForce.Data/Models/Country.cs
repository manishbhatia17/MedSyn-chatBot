using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class Country
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Territory Territory { get; set; }

        public virtual Region Region { get; set; }
    }
}
