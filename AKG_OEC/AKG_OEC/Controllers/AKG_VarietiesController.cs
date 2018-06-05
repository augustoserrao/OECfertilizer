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

        // GET: AKG_Varieties
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Variety.Include(v => v.Crop);
            return View(await oECContext.ToListAsync());
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

            return View(variety);
        }

        // POST: AKG_Varieties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
