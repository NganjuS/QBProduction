using System;
using System.Web.Mvc;
using QBProduction.Web.Models;
using QBProduction.Web.Helpers;
using System.Linq;
using NHibernate.Linq;

namespace QBProduction.Web.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports/RawMaterials
        public ActionResult RawMaterials(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            using (var session = NHibernateHelper.OpenSession())
            {
                var bomRuns = session.Query<BomRun>()
                    .Where(b => b.bomrundate >= startDate && b.bomrundate <= endDate)
                    .Fetch(b => b._bomrunsitems)
                    .OrderBy(b => b.bomrundate)
                    .ToList();

                ViewBag.StartDate = startDate.Value;
                ViewBag.EndDate = endDate.Value;

                return View(bomRuns);
            }
        }

        // GET: Reports/ProductionSummary
        public ActionResult ProductionSummary(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            using (var session = NHibernateHelper.OpenSession())
            {
                var bomRuns = session.Query<BomRun>()
                    .Where(b => b.bomrundate >= startDate && b.bomrundate <= endDate)
                    .OrderBy(b => b.bomrundate)
                    .ToList();

                // Group by product for summary
                var summary = bomRuns
                    .GroupBy(b => b.productionitem)
                    .Select(g => new
                    {
                        Product = g.Key,
                        TotalQuantity = g.Sum(b => b.totalqtyproduced),
                        TotalValue = g.Sum(b => b.totalvalue),
                        BatchCount = g.Count()
                    })
                    .ToList();

                ViewBag.StartDate = startDate.Value;
                ViewBag.EndDate = endDate.Value;
                ViewBag.Summary = summary;

                return View(bomRuns);
            }
        }
    }
}
