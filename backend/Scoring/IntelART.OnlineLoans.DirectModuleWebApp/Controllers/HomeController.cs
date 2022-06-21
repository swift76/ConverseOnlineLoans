using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IntelART.OnlineLoans.DirectModuleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
