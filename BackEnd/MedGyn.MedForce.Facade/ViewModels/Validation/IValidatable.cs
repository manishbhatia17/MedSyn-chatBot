namespace MedGyn.MedForce.Facade.ViewModels.Validation
{
    public interface IValidatable
    {
        bool IsValid { get; }
        bool IsInvalid { get; }
        void ClearValidationErrors();
    }
}
