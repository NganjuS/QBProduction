using System;
using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Helpers;
using System.Linq;
using NHibernate.Linq;

namespace QBProduction.Web.Controllers
{
    public class ProductionController : Controller
    {
        // GET: Production
        public ActionResult Index()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var bomRuns = session.Query<BomRun>()
                    .OrderByDescending(b => b.bomrundate)
                    .Take(100)
                    .ToList();

                return View(bomRuns);
            }
        }

        // GET: Production/StartProduction
        public ActionResult StartProduction(int? bomId)
        {
            if (bomId.HasValue)
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    var bom = session.Query<Boms>()
                        .Where(b => b.Id == bomId.Value)
                        .Fetch(b => b._bomitems)
                        .FirstOrDefault();

                    if (bom == null)
                        return HttpNotFound();

                    ViewBag.Bom = bom;
                }
            }

            return View();
        }

        // POST: Production/StartProduction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartProduction(BomRun bomRun)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var session = NHibernateHelper.OpenSession())
                    using (var transaction = session.BeginTransaction())
                    {
                        bomRun.bomrundate = DateTime.Now;
                        bomRun.batchstatus = "Pending";

                        // Generate unique reference number
                        var settings = session.Query<BomSettings>().FirstOrDefault();
                        if (settings != null)
                        {
                            settings.bomrefno++;
                            bomRun.bomrunref = settings.bomcode + settings.bomrefno.ToString("D6");
                            session.Update(settings);
                        }
                        else
                        {
                            bomRun.bomrunref = "PROD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        }

                        session.Save(bomRun);
                        transaction.Commit();
                    }

                    TempData["Success"] = "Production batch started successfully";
                    return RedirectToAction("Index");
                }

                return View(bomRun);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(bomRun);
            }
        }

        // GET: Production/BatchListing
        public ActionResult BatchListing()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var batches = session.Query<BomRun>()
                    .OrderByDescending(b => b.bomrundate)
                    .ToList();

                return View(batches);
            }
        }

        // GET: Production/BatchDetails/5
        public ActionResult BatchDetails(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var bomRun = session.Query<BomRun>()
                    .Where(b => b.Id == id)
                    .Fetch(b => b._bomrunsitems)
                    .FirstOrDefault();

                if (bomRun == null)
                    return HttpNotFound();

                return View(bomRun);
            }
        }
    }
}
