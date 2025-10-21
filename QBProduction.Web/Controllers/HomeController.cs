using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Data;
using System.Linq;

namespace QBProduction.Web.Controllers
{
    public class HomeController : Controller
    {
        private QBProductionContext db = new QBProductionContext();

        // GET: Home
        public ActionResult Index()
        {
            // Display list of BOMs similar to MainWindow
            var boms = db.Boms
                .Where(b => b.isactive)
                .OrderByDescending(b => b.createdon)
                .ToList();

            return View(boms);
        }

        public ActionResult About()
        {
            ViewBag.Message = "QuickBooks Production Module";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
