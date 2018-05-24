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
    public class AKG_FertilizerController : Controller
    {
        private readonly OECContext _context;

        public AKG_FertilizerController(OECContext context)
        {
            _context = context;
        }

        // GET: AKG_Fertilizer
        // This action shows index view
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fertilizer.ToListAsync());
        }

        // GET: AKG_Fertilizer/Details/5
        // This action reads information about a fertilizer from the database and shows it in detail view 
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizer
                .SingleOrDefaultAsync(m => m.FertilizerName == id);
            if (fertilizer == null)
            {
                return NotFound();
            }

            return View(fertilizer);
        }

        // GET: AKG_Fertilizer/Create
        // This action shows create view
        public IActionResult Create()
        {
            return View();
        }

        // POST: AKG_Fertilizer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // This action creates new fertilizer item in database and shows previous view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FertilizerName,Oecproduct,Liquid")] Fertilizer fertilizer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fertilizer);
        }

        // GET: AKG_Fertilizer/Edit/5
        // This action reads fertilizer information from database and shows it in edit view
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizer.SingleOrDefaultAsync(m => m.FertilizerName == id);
            if (fertilizer == null)
            {
                return NotFound();
            }
            return View(fertilizer);
        }

        // POST: AKG_Fertilizer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // This action updates fertilizer information in database and returns to previous view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FertilizerName,Oecproduct,Liquid")] Fertilizer fertilizer)
        {
            if (id != fertilizer.FertilizerName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FertilizerExists(fertilizer.FertilizerName))
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
            return View(fertilizer);
        }

        // GET: AKG_Fertilizer/Delete/5
        // This action shows delete view
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fertilizer = await _context.Fertilizer
                .SingleOrDefaultAsync(m => m.FertilizerName == id);
            if (fertilizer == null)
            {
                return NotFound();
            }

            return View(fertilizer);
        }

        // POST: AKG_Fertilizer/Delete/5
        // This action deletes fertilizer from database and returns to previous view 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string fertilizerName)
        {
            // Deleting from TreatmentFertilizer table because of foreign key constraints
            var treatments = _context.TreatmentFertilizer.Where(m => m.FertilizerName == fertilizerName)
                                          .Select(m => new { m.TreatmentFertilizerId }); ;
            foreach (var treatment in treatments)
                _context.TreatmentFertilizer.Remove(await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == treatment.TreatmentFertilizerId));

            var fertilizer = await _context.Fertilizer.SingleOrDefaultAsync(m => m.FertilizerName == fertilizerName);
            _context.Fertilizer.Remove(fertilizer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FertilizerExists(string id)
        {
            return _context.Fertilizer.Any(e => e.FertilizerName == id);
        }
    }
}
