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
    public class AKG_VarietiesController : Controller
    {
        private readonly OECContext _context;

        public AKG_VarietiesController(OECContext context)
        {
            _context = context;
        }

        private void ResetCookies()
        {
            Response.Cookies.Delete("cropId_Variety");
            Response.Cookies.Delete("cropName_Variety");
        }

        // GET: AKG_Varieties
        public async Task<IActionResult> Index(int? cropId, string cropName)
        {
            var oECContext = _context.Variety.Include(v => v.Crop);
            var varieties = await oECContext.ToListAsync();

            if (cropId != null)
            {
                ResetCookies();
                Response.Cookies.Append("cropId_Variety", cropId.ToString());

                if (cropName == null || cropName == "")
                    cropName = (await _context.Crop.SingleOrDefaultAsync(p => p.CropId == cropId)).Name;

                Response.Cookies.Append("cropName_Variety", cropName);
                ViewBag.cropName = cropName;
                
                return View(varieties.Where(p => p.CropId == cropId).OrderBy(p => p.Name));
            }
            else if (Request.Cookies["cropId_Variety"] != null)
            {
                ViewBag.cropName = Request.Cookies["cropName_Variety"];

                return View(varieties.Where(p => p.CropId == Convert.ToInt32(Request.Cookies["cropId"])).OrderBy(p => p.Name));
            }

            TempData["message"] = "Select a Crop to see its Varieties";
            
            return RedirectToAction("Index", "AKG_Crops");
        }

        // GET: AKG_Varieties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            return View(variety);
        }

        // GET: AKG_Varieties/Create
        public IActionResult Create()
        {
            ViewBag.cropName = Request.Cookies["cropName_Variety"];

            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId");
            return View();
        }

        // POST: AKG_Varieties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (ModelState.IsValid)
            {
                variety.CropId = Convert.ToInt32(Request.Cookies["cropId_Variety"]);
                _context.Add(variety);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // GET: AKG_Varieties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            ViewBag.cropName = Request.Cookies["cropName_Variety"];

            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // POST: AKG_Varieties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (id != variety.VarietyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variety);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VarietyExists(variety.VarietyId))
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
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "CropId", variety.CropId);
            return View(variety);
        }

        // GET: AKG_Varieties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);
            if (variety == null)
            {
                return NotFound();
            }

            ViewBag.cropName = Request.Cookies["cropName_Variety"];

            return View(variety);
        }

        // POST: AKG_Varieties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deleting from other tables because of foreign key constraints
            var plots = _context.Plot.Where(m => m.VarietyId == id)
                                          .Select(m => new { m.PlotId });

            foreach (var plot in plots)
            {
                var treatments = _context.Treatment.Where(m => m.PlotId == id)
                                              .Select(m => new { m.TreatmentId });

                foreach (var treatment in treatments)
                {
                    var treatmentFertilizers = _context.TreatmentFertilizer.Where(m => m.TreatmentId == treatment.TreatmentId)
                                              .Select(m => new { m.TreatmentFertilizerId });

                    foreach (var fertilizer in treatmentFertilizers)
                        _context.TreatmentFertilizer.Remove(await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == fertilizer.TreatmentFertilizerId));

                    _context.Treatment.Remove(await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == treatment.TreatmentId));
                }

                _context.Plot.Remove(await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == plot.PlotId));
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            _context.Variety.Remove(variety);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VarietyExists(int id)
        {
            return _context.Variety.Any(e => e.VarietyId == id);
        }
    }
}
