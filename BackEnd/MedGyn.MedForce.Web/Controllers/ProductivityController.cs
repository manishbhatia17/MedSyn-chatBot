using Microsoft.AspNetCore.Mvc;

namespace MedGyn.MedForce.Web.Controllers
{
    public class ProductivityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
