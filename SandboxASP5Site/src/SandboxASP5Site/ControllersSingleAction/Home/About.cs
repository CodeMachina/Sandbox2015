using Microsoft.AspNet.Mvc;

namespace SandboxASP5Site.ControllersSingleAction.Home
{
    public class About : Controller
    {
        public IActionResult Execute()
        {
            return Json(new { success = "i am an about single action controller!" });
        }
    }
}
