using Microsoft.AspNet.Mvc;

namespace ProductNames.ControllersSingleAction {
    [Route("/")]
    public class Get : Controller {
        [HttpGet]
        public IActionResult Execute() {
            return View("~/Views/ReadMe.cshtml");
        }

        [HttpGet("foo/bar")]
        public IActionResult Flubber(string what) {
            return Json(new {blah = "blah" } );
        }
    }
}
