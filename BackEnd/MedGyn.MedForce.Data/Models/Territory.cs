using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class Territory
    {
        public virtual int Id { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string Type { get; set; }
        public virtual IList<State> States { get; set; }
        public virtual IList<Country> Countries { get; set; }

    }
}
