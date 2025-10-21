using System;
using System.Data.Entity;
using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Data;
using System.Linq;

namespace QBProduction.Web.Controllers
{
    public class BomController : Controller
    {
        private QBProductionContext db = new QBProductionContext();

        // GET: Bom
        public ActionResult Index()
        {
            var boms = db.Boms
                .Where(b => b.isactive)
                .OrderByDescending(b => b.createdon)
                .ToList();

            return View(boms);
        }

        // GET: Bom/Details/5
        public ActionResult Details(int id)
        {
            var bom = db.Boms
                .Include(b => b.BomItems)
                .FirstOrDefault(b => b.Id == id);

            if (bom == null)
                return HttpNotFound();

            return View(bom);
        }

        // GET: Bom/Edit/5
        public ActionResult Edit(int id)
        {
            var bom = db.Boms
                .Include(b => b.BomItems)
                .FirstOrDefault(b => b.Id == id);

            if (bom == null)
                return HttpNotFound();

            return View(bom);
        }

        // POST: Bom/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Boms bom)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bom.modifiedon = DateTime.Now;
                    db.Entry(bom).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(bom);
            }
            catch
            {
                return View(bom);
            }
        }

        // GET: Bom/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bom/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Boms bom)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bom.createdon = DateTime.Now;
                    bom.modifiedon = DateTime.Now;
                    bom.isactive = true;
                    db.Boms.Add(bom);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(bom);
            }
            catch
            {
                return View(bom);
            }
        }

        // POST: Bom/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var bom = db.Boms.Find(id);
                if (bom != null)
                {
                    bom.isactive = false;
                    bom.modifiedon = DateTime.Now;
                    db.Entry(bom).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Bom/LoadNew - Load BOMs from QuickBooks
        public ActionResult LoadNew()
        {
            try
            {
                // This would integrate with QuickBooks to load new BOMs
                // For now, redirect to index
                TempData["Message"] = "QuickBooks integration pending";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
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
