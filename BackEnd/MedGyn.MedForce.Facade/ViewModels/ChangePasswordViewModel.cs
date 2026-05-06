using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedGyn.MedForce.Facade.ViewModels
{
    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel()
        {
            Errors = new List<string>();
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public List<string> Errors { get; set; }
        public bool Success => Errors.Count == 0;
    }
}

