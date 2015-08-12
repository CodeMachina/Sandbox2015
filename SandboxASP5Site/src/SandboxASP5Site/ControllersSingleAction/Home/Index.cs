using Microsoft.AspNet.Mvc;

namespace SandboxASP5Site.ControllersSingleAction.Home
{
    public class Index : Controller
    {
        public IActionResult Execute()
        {
            return View("~/ControllersSingleAction/Home/Index.cshtml");
        }
    }
}
