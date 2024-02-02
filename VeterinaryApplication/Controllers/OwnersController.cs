using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VeterinaryApplication.Models;
using VeterinaryApplication.Repository;
using VeterinaryApplication.Services;

namespace VeterinaryApplication.Controllers
{
    public class OwnersController : Controller
    {
        private IMemoryCache _memoryCache;

        private IOwnerService ownerService;

        public OwnersController(IOwnerService ownerService, IMemoryCache memoryCache)
        {
            this.ownerService = ownerService;
            this._memoryCache = memoryCache;
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            {
                List<Owner> owners;

                //owners = ownerService.GetAllWithInclude(x => x.Pets).ToList();

                if (!_memoryCache.TryGetValue("owners", out owners))
                {
                    owners = ownerService
                        .GetAllWithInclude(x => x.Pets).ToList();

                    MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions();
                    cacheOptions.SetPriority(CacheItemPriority.Low);
                    cacheOptions.SetSlidingExpiration(new TimeSpan(0, 0, 15));
                    cacheOptions.SetAbsoluteExpiration(new TimeSpan(0, 0, 30));

                    _memoryCache.Set("owners", owners, cacheOptions);
                }

                return View(owners);
            }
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = ownerService.GetAllWithInclude(x => x.Pets)
                .FirstOrDefault(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OwnerId,FirstName,LastName,Years")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                ownerService.Add(owner);
                _memoryCache.Remove("owners");
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        // GET: Owners/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = ownerService.GetById(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,FirstName,LastName,Years")] Owner owner)
        {
            if (id != owner.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ownerService.Update(owner);
                    _memoryCache.Remove("owners");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.OwnerId))
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
            return View(owner);
        }

        // GET: Owners/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = ownerService.GetById(id);


            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = ownerService.GetById(id);
            if (owner != null)
            {
                ownerService.Delete(owner.OwnerId);
                _memoryCache.Remove("owners");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(int id)
        {
            var ownerExist = ownerService.GetById(id);
            if (ownerExist == null)
            {
                return false;
            }
            return true;
        }
    }
}
