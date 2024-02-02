using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Drawing;
using VeterinaryApplication.Data;
using VeterinaryApplication.Models;
using VeterinaryApplication.Services;

namespace VeterinaryApplication.Controllers
{
    public class PetsController : Controller
    {
        private IMemoryCache _memoryCache;
        private IPetService petService;

        public PetsController(IPetService petService, IMemoryCache memoryCache)
        {
            this.petService = petService;
            this._memoryCache = memoryCache;
        }

        // GET: Pets
        public async Task<IActionResult> Index()
        {
            List<Pet> pets;

            if (!_memoryCache.TryGetValue("pets", out pets))
            {
                /* pets = await _context.Pets
                      .Include(x => x.Vaccines)  
                      .Include(x => x.Owner)     
                      .ToListAsync();

              pets = await _context.Pets.Include(x=> x.Owner).ToListAsync();*/
                pets = petService.GetAllWithInclude(x => x.Owner).ToList();

                pets = petService.GetAllWithInclude(v => v.Vaccines).ToList();


                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                cacheOptions.SetPriority(CacheItemPriority.Low);
                cacheOptions.SetSlidingExpiration(new TimeSpan(0, 0, 15));
                cacheOptions.SetAbsoluteExpiration(new TimeSpan(0, 0, 30));

                _memoryCache.Set("pets", pets, cacheOptions);
            }

            return View(pets);
        }

        // GET: Pets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /* var pet = await _context.Pets
                 .Include(p => p.Owner)
                 .Include(p => p.Vaccines)
                 .FirstOrDefaultAsync(m => m.PetId == id);*/

            var pets = petService.GetAllWithInclude(x => x.Owner);
            pets = petService.GetAllWithInclude(v => v.Vaccines).ToList();

            var pet = petService.GetAllWithInclude(x => x.Vaccines)
                .FirstOrDefault(m => m.PetId == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // GET: Pets/Create
        public IActionResult Create()
        {
            /* ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "FirstName");
             ViewData["VaccineId"] = new MultiSelectList(_context.Vaccines, "VaccineId", "Name");*/

            return View();
        }

        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PetId,Name,Age,OwnerId,VaccinesParams")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                /*  pet.Vaccines = _context.Vaccines.Where(x => pet.VaccinesParams.Contains(x.VaccineId)).ToList();

                  _context.Add(pet);
                  await _context.SaveChangesAsync();*/
                petService.Add(pet);
                _memoryCache.Remove("pets");
                return RedirectToAction(nameof(Index));

            }
            /*  ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "FirstName", pet.OwnerId);
              ViewData["VaccineId"] = new MultiSelectList(_context.Vaccines, "VaccineId", "Name", pet.VaccinesParams);*/

            return View(pet);
        }

        // GET: Pets/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /* var pet = await _context.Pets
                 .Include(x => x.Vaccines)
                 .FirstOrDefaultAsync(x => x.PetId == id);*/

            // New  implementation to be tested.
            //var pet = petService
            //    .GetAllWithInclude(v => v.Vaccines).ToList()
            //.Where(p => p.PetId == id);

            var pet = petService.GetById(id);

            if (pet == null)
            {
                return NotFound();
            }
            /* ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "FirstName", pet.OwnerId);
             ViewData["VaccineId"] = new MultiSelectList(_context.Vaccines, "VaccineId", "Name", pet.Vaccines.Select(x => x.VaccineId));*/


            return View(pet);
        }

        // POST: Pets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PetId,Name,Age,OwnerId,VaccinesParams")] Pet pet)
        {
            if (id != pet.PetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /* var existingPet = await _context.Pets
                         .Include(p => p.Vaccines)
                         .FirstOrDefaultAsync(p => p.PetId == id);

                     if (existingPet == null)
                     {
                         return NotFound();
                     }

                     existingPet.Name = pet.Name;
                     existingPet.Age = pet.Age;
                     existingPet.OwnerId = pet.OwnerId;

                     existingPet.Vaccines = _context.Vaccines
                         .Where(v => pet.VaccinesParams.Contains(v.VaccineId))
                         .ToList();

                     _context.Update(existingPet);
                     _memoryCache.Remove("pets");
                     await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));*/

                    petService.Update(pet);
                    _memoryCache.Remove("pets");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.PetId))
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

            // Populate ViewData for dropdowns
            /* ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "FirstName", pet.OwnerId);
             ViewData["VaccineId"] = new MultiSelectList(_context.Vaccines, "VaccineId", "Name", pet.VaccinesParams);*/

            // Return the view with the modified pet
            return View(pet);
        }

        // GET: Pets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*var pet = await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.PetId == id);*/

            var pet = petService.GetById(id);

            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var pet = await _context.Pets.FindAsync(id);
            var pet = petService.GetById(id);
            if (pet != null)
            {
                /* _context.Pets.Remove(pet);
                 await _context.SaveChangesAsync();*/

                petService.Delete(pet.PetId);
                _memoryCache.Remove("pets");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PetExists(int id)
        {
            // return _context.Pets.Any(e => e.PetId == id);

            var petExist = petService.GetById(id);
            if (petExist == null)
            {
                return false;
            }
            return true;
        }
    }
}
