using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IntelART.OnlineLoans.BankModuleWebApp.Controllers
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
