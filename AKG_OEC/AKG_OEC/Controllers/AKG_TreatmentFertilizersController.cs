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
    public class AKG_TreatmentFertilizersController : Controller
    {
        private readonly OECContext _context;

        public AKG_TreatmentFertilizersController(OECContext context)
        {
            _context = context;
        }

        // GET: AKG_TreatmentFertilizers
        public async Task<IActionResult> Index(int? treatmentId)
        {
            int? curTreatmentId;

            var oECContext = _context.TreatmentFertilizer.Include(t => t.FertilizerNameNavigation).Include(t => t.Treatment);
            var treatmentFertilizers = (await oECContext.ToListAsync());

            if (treatmentId != null)
            {
                Response.Cookies.Delete("treatmentId");
                Response.Cookies.Append("treatmentId", treatmentId.ToString());

                curTreatmentId = treatmentId;
            }
            else if (Request.Cookies["plotId"] != null)
            {
                curTreatmentId = Convert.ToInt32(Request.Cookies["treatmentId"]);
            }
            else
            {
                TempData["message"] = "Select a Treatment to see its Fertilizer Composition";

                return RedirectToAction("Index", "AKG_Treatments");
            }

            return View(treatmentFertilizers.Where(p => p.TreatmentId == curTreatmentId).OrderBy(p => p.FertilizerName));
        }

        // GET: AKG_TreatmentFertilizers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // GET: AKG_TreatmentFertilizers/Create
        public IActionResult Create()
        {
            List<string> rateMetric = new List<string> { "Gal", "LB" };

            ViewData["RateMetric"] = new SelectList(rateMetric);
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(p => p.FertilizerName), "FertilizerName", "FertilizerName");
   
            return View();
        }

        // POST: AKG_TreatmentFertilizers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentFertilizerId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            List<string> rateMetric = new List<string> { "Gal", "LB" };

            if (ModelState.IsValid)
            {
                treatmentFertilizer.TreatmentId = Convert.ToInt32(Request.Cookies["treatmentId"]);
                _context.Add(treatmentFertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["RateMetric"] = new SelectList(rateMetric);
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(p => p.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);

            return View(treatmentFertilizer);
        }

        // GET: AKG_TreatmentFertilizers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }
            List<string> rateMetric = new List<string> { "Gal", "LB" };

            ViewData["RateMetric"] = new SelectList(rateMetric);
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(p => p.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);

            return View(treatmentFertilizer);
        }

        // POST: AKG_TreatmentFertilizers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentFertilizerId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            if (id != treatmentFertilizer.TreatmentFertilizerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    treatmentFertilizer.TreatmentId = Convert.ToInt32(Request.Cookies["treatmentId"]);
                    _context.Update(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentFertilizerExists(treatmentFertilizer.TreatmentFertilizerId))
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
            List<string> rateMetric = new List<string> { "Gal", "LB" };

            ViewData["RateMetric"] = new SelectList(rateMetric);
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(p => p.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);

            return View(treatmentFertilizer);
        }

        // GET: AKG_TreatmentFertilizers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // POST: AKG_TreatmentFertilizers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            _context.TreatmentFertilizer.Remove(treatmentFertilizer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentFertilizerExists(int id)
        {
            return _context.TreatmentFertilizer.Any(e => e.TreatmentFertilizerId == id);
        }
    }
}
