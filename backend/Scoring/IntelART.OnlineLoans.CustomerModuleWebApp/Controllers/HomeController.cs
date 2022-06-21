using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IntelART.OnlineLoans.BankModuleWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Index2()
        {
            return View();
        }
    }
}
