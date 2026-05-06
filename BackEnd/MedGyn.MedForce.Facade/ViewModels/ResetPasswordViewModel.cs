using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
