using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Tayko.co.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult HandleError(int error)
        {
            // retrieve the status code
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // switch handler matches code to correct view
            switch (error)
            {
                case 404:
                    return View("404");
                default:
                    return View("500");
            }
        }
    }
}