using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IntelART.OnlineLoans.DirectModuleWebApp.Controllers
{
    public class SharedController : Controller
    {
        public async Task<IActionResult> Unsupported()
        {
            ViewBag.DisableIERedirect = true;
            return View();
        }
    }
}
