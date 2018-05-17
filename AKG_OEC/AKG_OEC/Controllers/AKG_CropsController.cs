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
    public class AKG_CropsController : Controller
    {
        private readonly OECContext _context;

        public AKG_CropsController(OECContext context)
        {
            _context = context;
        }

        // GET: AKG_Crops
        public async Task<IActionResult> Index()
        {
            return View(await _context.Crop.ToListAsync());
        }

        // GET: AKG_Crops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop
                .SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }

            return View(crop);
        }

        // GET: AKG_Crops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AKG_Crops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CropId,Name,Image")] Crop crop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(crop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(crop);
        }

        // GET: AKG_Crops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop.SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }
            return View(crop);
        }

        // POST: AKG_Crops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CropId,Name,Image")] Crop crop)
        {
            if (id != crop.CropId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(crop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CropExists(crop.CropId))
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
            return View(crop);
        }

        // GET: AKG_Crops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crop = await _context.Crop
                .SingleOrDefaultAsync(m => m.CropId == id);
            if (crop == null)
            {
                return NotFound();
            }

            return View(crop);
        }

        // POST: AKG_Crops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var varieties = _context.Variety.Where(m => m.CropId == id)
                                          .Select(m => new { m.VarietyId }); ;
            foreach (var variety in varieties)
                _context.Variety.Remove(await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == variety.VarietyId));

            var crop = await _context.Crop.SingleOrDefaultAsync(m => m.CropId == id);
            _context.Crop.Remove(crop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CropExists(int id)
        {
            return _context.Crop.Any(e => e.CropId == id);
        }
    }
}
