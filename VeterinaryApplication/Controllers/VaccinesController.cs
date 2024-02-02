using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VeterinaryApplication.Data;
using VeterinaryApplication.Models;
using VeterinaryApplication.Services;

namespace VeterinaryApplication.Controllers
{
    public class VaccinesController : Controller
    {
        private IMemoryCache _memoryCache;
        private IVaccineService vaccineService;

        public VaccinesController(IVaccineService vaccineService, IMemoryCache memoryCache)
        {
            this.vaccineService = vaccineService;
            this._memoryCache = memoryCache;

        }

        // GET: Vaccines
        public async Task<IActionResult> Index()
        {
            List<Vaccine> vaccines;

            if (!_memoryCache.TryGetValue("vaccines", out vaccines))
            {
                vaccines = vaccineService.GetAllWithInclude(x => x.Pets).ToList();

                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetPriority(CacheItemPriority.Low);
                cacheOptions.SetSlidingExpiration(new TimeSpan(0, 0, 15));
                cacheOptions.SetAbsoluteExpiration(new TimeSpan(0, 0, 30));

                _memoryCache.Set("vaccines", vaccines, cacheOptions);
            }

            return View(vaccines);
        }

        // GET: Vaccines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaccine = vaccineService.GetAllWithInclude(x => x.Pets)
                .FirstOrDefault(m => m.VaccineId == id);
            if (vaccine == null)
            {
                return NotFound();
            }

            return View(vaccine);
        }

        // GET: Vaccines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vaccines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VaccineId,Name")] Vaccine vaccine)
        {
            if (ModelState.IsValid)
            {
                vaccineService.Add(vaccine);
                // await vaccineService.SaveChangesAsync();
                _memoryCache.Remove("vaccines");
                return RedirectToAction(nameof(Index));
            }
            return View(vaccine);
        }

        // GET: Vaccines/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaccine = vaccineService.GetById(id);
            if (vaccine == null)
            {
                return NotFound();
            }
            return View(vaccine);
        }

        // POST: Vaccines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VaccineId,Name")] Vaccine vaccine)
        {
            if (id != vaccine.VaccineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    vaccineService.Update(vaccine);
                    _memoryCache.Remove("vaccines");
                    // await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VaccineExists(vaccine.VaccineId))
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
            return View(vaccine);
        }

        // GET: Vaccines/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var vaccine = await _context.Vaccines
            //    .FirstOrDefaultAsync(m => m.VaccineId == id);

            var vaccine = vaccineService.GetById(id);

            if (vaccine == null)
            {
                return NotFound();
            }

            return View(vaccine);
        }

        // POST: Vaccines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vaccine = vaccineService.GetById(id);
            if (vaccine != null)
            {
                vaccineService.Delete(vaccine.VaccineId);
                // await _context.SaveChangesAsync();
                _memoryCache.Remove("vaccines");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VaccineExists(int id)
        {
            var vaccineExist = vaccineService.GetById(id);
            if (vaccineExist == null)
            {
                return false;
            }
            return true;
        }
    }
}
