using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Facade.ViewModels.Validation
{
    public interface IValidationError
    {
        string FieldId { get; }
        List<string> Messages { get; }
    }
}
