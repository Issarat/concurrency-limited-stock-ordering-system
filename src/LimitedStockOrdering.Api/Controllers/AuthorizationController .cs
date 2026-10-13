using Microsoft.AspNetCore.Mvc;

namespace LimitedStockOrdering.Api.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
