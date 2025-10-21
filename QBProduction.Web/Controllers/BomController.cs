using System;
using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Helpers;
using System.Linq;
using NHibernate.Linq;

namespace QBProduction.Web.Controllers
{
    public class BomController : Controller
    {
        // GET: Bom
        public ActionResult Index()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var boms = session.Query<Boms>()
                    .Where(b => b.isactive)
                    .OrderByDescending(b => b.createdon)
                    .ToList();

                return View(boms);
            }
        }

        // GET: Bom/Details/5
        public ActionResult Details(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var bom = session.Query<Boms>()
                    .Where(b => b.Id == id)
                    .FirstOrDefault();

                if (bom == null)
                    return HttpNotFound();

                return View(bom);
            }
        }

        // GET: Bom/Edit/5
        public ActionResult Edit(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var bom = session.Query<Boms>()
                    .Where(b => b.Id == id)
                    .Fetch(b => b._bomitems)
                    .FirstOrDefault();

                if (bom == null)
                    return HttpNotFound();

                return View(bom);
            }
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
                    using (var session = NHibernateHelper.OpenSession())
                    using (var transaction = session.BeginTransaction())
                    {
                        bom.modifiedon = DateTime.Now;
                        session.Update(bom);
                        transaction.Commit();
                    }

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
                    using (var session = NHibernateHelper.OpenSession())
                    using (var transaction = session.BeginTransaction())
                    {
                        bom.createdon = DateTime.Now;
                        bom.modifiedon = DateTime.Now;
                        bom.isactive = true;
                        session.Save(bom);
                        transaction.Commit();
                    }

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
                using (var session = NHibernateHelper.OpenSession())
                using (var transaction = session.BeginTransaction())
                {
                    var bom = session.Get<Boms>(id);
                    if (bom != null)
                    {
                        bom.isactive = false;
                        bom.modifiedon = DateTime.Now;
                        session.Update(bom);
                        transaction.Commit();
                    }
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
    }
}
