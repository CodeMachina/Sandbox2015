using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace ReactSite.Pages.Home
{
    public class HomeController : Controller
    {
        public ActionResult Index() {
            return View("~/Pages/Home/Home.cshtml");
        }
    }
}
