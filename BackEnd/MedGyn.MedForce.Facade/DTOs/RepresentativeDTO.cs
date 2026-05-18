using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.DTOs
{
    public class RepresentativeDTO
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }
    }
}
