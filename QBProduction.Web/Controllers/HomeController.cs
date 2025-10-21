using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Helpers;
using System.Linq;

namespace QBProduction.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Display list of BOMs similar to MainWindow
            using (var session = NHibernateHelper.OpenSession())
            {
                var boms = session.Query<Boms>()
                    .Where(b => b.isactive)
                    .OrderByDescending(b => b.createdon)
                    .ToList();

                return View(boms);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "QuickBooks Production Module";
            return View();
        }
    }
}
