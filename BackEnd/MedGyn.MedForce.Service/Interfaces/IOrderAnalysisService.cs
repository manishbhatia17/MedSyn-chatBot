using MedGyn.MedForce.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MedGyn.MedForce.Service.Interfaces
{
    public interface IOrderAnalysisService
    {
        Task<IDictionary<string, OrderAnalysisContract>> AnalizeOrder(string FileUrl);
    }
}
