using System;
using System.Data.Entity;
using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Data;
using System.Linq;

namespace QBProduction.Web.Controllers
{
    public class ProductionController : Controller
    {
        private QBProductionContext db = new QBProductionContext();

        // GET: Production
        public ActionResult Index()
        {
            var bomRuns = db.BomRuns
                .OrderByDescending(b => b.bomrundate)
                .Take(100)
                .ToList();

            return View(bomRuns);
        }

        // GET: Production/StartProduction
        public ActionResult StartProduction(int? bomId)
        {
            if (bomId.HasValue)
            {
                var bom = db.Boms
                    .Include(b => b.BomItems)
                    .FirstOrDefault(b => b.Id == bomId.Value);

                if (bom == null)
                    return HttpNotFound();

                ViewBag.Bom = bom;
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
                    bomRun.bomrundate = DateTime.Now;
                    bomRun.batchstatus = "Pending";

                    // Generate unique reference number
                    var settings = db.BomSettings.FirstOrDefault();
                    if (settings != null)
                    {
                        settings.bomrefno++;
                        bomRun.bomrunref = settings.bomcode + settings.bomrefno.ToString("D6");
                        db.Entry(settings).State = EntityState.Modified;
                    }
                    else
                    {
                        bomRun.bomrunref = "PROD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }

                    db.BomRuns.Add(bomRun);
                    db.SaveChanges();

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
            var batches = db.BomRuns
                .OrderByDescending(b => b.bomrundate)
                .ToList();

            return View(batches);
        }

        // GET: Production/BatchDetails/5
        public ActionResult BatchDetails(int id)
        {
            var bomRun = db.BomRuns
                .Include(b => b.BomRunItems)
                .FirstOrDefault(b => b.Id == id);

            if (bomRun == null)
                return HttpNotFound();

            return View(bomRun);
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
