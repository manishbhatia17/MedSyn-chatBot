using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedGyn.MedForce.Data.Models
{
    public class Representative
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }
    }
}
