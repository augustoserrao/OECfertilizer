using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AKG_OEC.Models;

namespace AKG_OEC.Controllers
{
    public class AKG_TreatmentsController : Controller
    {
        private const string NO_FERTILIZER = "no fertilizer";

        private readonly OECContext _context;

        public AKG_TreatmentsController(OECContext context)
        {
            _context = context;
        }

        // GET: AKG_Treatments
        public async Task<IActionResult> Index(int? plotId, string farmName)
        {
            int? curPlotId;

            var oECContext = _context.Treatment.Include(t => t.Plot);
            var treatments = (await oECContext.ToListAsync());

            if (plotId != null)
            {
                Response.Cookies.Delete("plotId");
                Response.Cookies.Append("plotId", plotId.ToString());

                curPlotId = plotId;
            }
            else if (Request.Cookies["plotId"] != null)
            {
                curPlotId = Convert.ToInt32(Request.Cookies["plotId"]);
            }
            else
            {
                TempData["message"] = "Select a Plot to see its Treatments";

                return RedirectToAction("Index", "AKG_Plot");
            }

            if ((farmName == null || farmName == "") && Request.Cookies["farmName"] != null)
            {
                int? farmId = _context.Plot.SingleOrDefault(p => p.PlotId == curPlotId).FarmId;
                string fName = _context.Farm.SingleOrDefault(p => p.FarmId == farmId).Name;
                ViewBag.farmName = fName;
                Response.Cookies.Append("farmName", fName);
            }
            else if (farmName != null && farmName != "")
            {
                ViewBag.farmName = farmName;
                Response.Cookies.Delete("farmName");
                Response.Cookies.Append("farmName", farmName);
            }
            else
            {
                ViewBag.farmName = Request.Cookies["farmName"];
            }

            foreach (var treatment in treatments.Where(p => p.PlotId == curPlotId))
            {
                var treatmentFertilizers = _context.TreatmentFertilizer.Where(p => p.TreatmentId == treatment.TreatmentId);

                if (treatmentFertilizers.Count() == 0 && treatment.Name != NO_FERTILIZER)
                {
                    treatment.Name = NO_FERTILIZER;
                    await Edit(treatment.TreatmentId, treatment);
                }
                else
                {
                    string newName = "";
                    foreach (TreatmentFertilizer treatmentFertilizer in treatmentFertilizers)
                    {
                        newName += treatmentFertilizer.FertilizerName;

                        if (treatmentFertilizer != treatmentFertilizers.Last())
                        {
                            newName += " + ";
                        }
                    }

                    if (newName == "")
                        newName = NO_FERTILIZER;

                    if (treatment.Name != newName)
                    {
                        treatment.Name = newName;
                        await Edit(treatment.TreatmentId, treatment);
                    }
                }
            }

            ViewBag.plotId = curPlotId;

            return View(treatments.Where(p => p.PlotId == curPlotId).OrderBy(p => p.Name));
        }

        // GET: AKG_Treatments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            ViewBag.farmName = Request.Cookies["farmName"];

            return View(treatment);
        }

        // GET: AKG_Treatments/Create
        public IActionResult Create()
        {
            ViewBag.farmName = Request.Cookies["farmName"];

            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId");
            return View();
        }

        // POST: AKG_Treatments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentId,Name,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                treatment.PlotId = Convert.ToInt32(Request.Cookies["plotId"]);
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: AKG_Treatments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            ViewBag.farmName = Request.Cookies["farmName"];

            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // POST: AKG_Treatments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.TreatmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: AKG_Treatments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            ViewBag.farmName = Request.Cookies["farmName"];

            return View(treatment);
        }

        // POST: AKG_Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deleting from Variety table because of foreign key constraints
            var treatmentFertilizers = _context.TreatmentFertilizer.Where(m => m.TreatmentId == id)
                                                                   .Select(m => new { m.TreatmentFertilizerId }); ;
            foreach (var treatmentFertilizer in treatmentFertilizers)
                _context.TreatmentFertilizer.Remove(await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == treatmentFertilizer.TreatmentFertilizerId));

            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            _context.Treatment.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(int id)
        {
            return _context.Treatment.Any(e => e.TreatmentId == id);
        }
    }
}
