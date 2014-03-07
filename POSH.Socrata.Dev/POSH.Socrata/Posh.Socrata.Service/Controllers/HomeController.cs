using System.Web.Mvc;

namespace Posh.Socrata.Service.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}