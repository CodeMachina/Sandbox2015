using Microsoft.AspNetCore.Mvc;

namespace ReactSite.Pages.Home {
    public class HomeController : Controller
    {
        public ActionResult Index() {
            return View("~/Pages/Home/Home.cshtml");
        }
    }
}
