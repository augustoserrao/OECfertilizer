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
    public class AKG_PlotController : Controller
    {
        private readonly OECContext _context;

        public AKG_PlotController(OECContext context)
        {
            _context = context;
        }

        private void ResetCookies()
        {
            Response.Cookies.Delete("cropId");
            Response.Cookies.Delete("varietyId");
            Response.Cookies.Delete("cropName");
            Response.Cookies.Delete("varietyName");
        }

        // GET: AKG_Plot
        public async Task<IActionResult> Index(int? cropId, string cropName, int? varietyId, string varietyName, int? showAll)
        {
            var oECContext = _context.Plot.Include(p => p.Farm).Include(p => p.Variety).Include(p => p.Treatment).Include(p => p.Variety.Crop);
            var plots = (await oECContext.ToListAsync()).OrderByDescending(p => p.DatePlanted);

            if (cropId != null)
            {
                ResetCookies();
                Response.Cookies.Append("cropId", cropId.ToString());
                Response.Cookies.Append("cropName", cropName);
                ViewBag.cropName = cropName;

                foreach (var plot in plots)
                {
                    if (plot.Variety == null)
                        plot.Variety = new Variety() { };
                }

                return View(plots.Where(p => p.Variety.CropId == cropId));
            }
            else if (varietyId != null)
            {
                ResetCookies();
                Response.Cookies.Append("varietyId", cropId.ToString());
                Response.Cookies.Append("varietyName", varietyName);
                ViewBag.varietyName = varietyName;

                return View(plots.Where(p => p.VarietyId == varietyId));
            }
            else if (showAll != null)
            {
                ResetCookies();
            }
            else if (Request.Cookies["cropId"] != null)
            {
                foreach (var plot in plots)
                {
                    if (plot.Variety == null)
                        plot.Variety = new Variety() { };
                }

                ViewBag.cropName = Request.Cookies["cropName"];

                return View(plots.Where(p => p.Variety.CropId == Convert.ToInt32(Request.Cookies["cropId"])));
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewBag.varietyName = Request.Cookies["varietyName"];
                return View(plots.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])));
            }

            return View(plots);
        }

        // GET: AKG_Plot order by farm
        public async Task<IActionResult> IndexOrderFarm()
        {
            var oECContext = _context.Plot.Include(p => p.Farm).Include(p => p.Variety).Include(p => p.Treatment).Include(p => p.Variety.Crop);
            var plots = (await oECContext.ToListAsync()).OrderBy(p => p.Farm.Name);

            if (Request.Cookies["cropId"] != null)
            {
                foreach (var plot in plots)
                {
                    if (plot.Variety == null)
                        plot.Variety = new Variety() { };
                }

                ViewBag.cropName = Request.Cookies["cropName"];

                return View("Index", plots.Where(p => p.Variety.CropId == Convert.ToInt32(Request.Cookies["cropId"])));
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewBag.varietyName = Request.Cookies["varietyName"];
                return View("Index", plots.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])));
            }
            
            return View("Index", plots);
        }

        // GET: AKG_Plot order by variety
        public async Task<IActionResult> IndexOrderVariety()
        {
            var oECContext = _context.Plot.Include(p => p.Farm).Include(p => p.Variety).Include(p => p.Treatment).Include(p => p.Variety.Crop);
            var plots = (await oECContext.ToListAsync());

            foreach (var plot in plots)
            {
                if (plot.Variety == null)
                    plot.Variety = new Variety() { Name = "" };
            }

            var ret = plots.OrderBy(p => p.Variety.Name);

            if (Request.Cookies["cropId"] != null)
            {
                ViewBag.cropName = Request.Cookies["cropName"];

                return View("Index", ret.Where(p => p.Variety.CropId == Convert.ToInt32(Request.Cookies["cropId"])));
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewBag.varietyName = Request.Cookies["varietyName"];
                return View("Index", ret.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])));
            }
            
            return View("Index", ret);
        }

        // GET: AKG_Plot order by CEC
        public async Task<IActionResult> IndexOrderCec()
        {
            var oECContext = _context.Plot.Include(p => p.Farm).Include(p => p.Variety).Include(p => p.Treatment).Include(p => p.Variety.Crop);
            var plots = (await oECContext.ToListAsync()).OrderBy(p => p.Cec);

            if (Request.Cookies["cropId"] != null)
            {
                foreach (var plot in plots)
                {
                    if (plot.Variety == null)
                        plot.Variety = new Variety() { };
                }

                ViewBag.cropName = Request.Cookies["cropName"];

                return View("Index", plots.Where(p => p.Variety.CropId == Convert.ToInt32(Request.Cookies["cropId"])));
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewBag.varietyName = Request.Cookies["varietyName"];
                return View("Index", plots.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])));
            }

            return View("Index", plots);
        }

        // GET: AKG_Plot/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // GET: AKG_Plot/Create
        public IActionResult Create()
        {
            ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(p => p.Name), "FarmId", "Name");

            if (Request.Cookies["cropId"] != null)
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.Where(p => p.CropId == Convert.ToInt32(Request.Cookies["cropId"])).OrderBy(p => p.Name), "VarietyId", "Name");
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])).OrderBy(p => p.Name), "VarietyId", "Name");
            }
            else
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.OrderBy(p => p.Name), "VarietyId", "Name");
            }

            return View();
        }

        // POST: AKG_Plot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: AKG_Plot/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(p => p.Name), "FarmId", "Name", plot.FarmId);

            if (Request.Cookies["cropId"] != null)
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.Where(p => p.CropId == Convert.ToInt32(Request.Cookies["cropId"])).OrderBy(p => p.Name), "VarietyId", "Name", plot.VarietyId);
            }
            else if (Request.Cookies["varietyId"] != null)
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.Where(p => p.VarietyId == Convert.ToInt32(Request.Cookies["varietyId"])).OrderBy(p => p.Name), "VarietyId", "Name", plot.VarietyId);
            }
            else
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.OrderBy(p => p.Name), "VarietyId", "Name", plot.VarietyId);
            }
            
            return View(plot);
        }

        // POST: AKG_Plot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlotExists(plot.PlotId))
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
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: AKG_Plot/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // POST: AKG_Plot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deleting from Variety table because of foreign key constraints
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

            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            _context.Plot.Remove(plot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
    }
}
