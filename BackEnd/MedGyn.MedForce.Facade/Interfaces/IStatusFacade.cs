using MedGyn.MedForce.Facade.ViewModels;

namespace MedGyn.MedForce.Facade.Interfaces
{
    public interface IStatusFacade
    {
        StatusViewModel GetStatus();
        StatusViewModel UpdateDatabase(StatusViewModel statusViewModel);
    }
}
