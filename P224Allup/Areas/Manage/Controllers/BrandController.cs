using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P224Allup.DAL;
using P224Allup.Extensions;
using P224Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P224Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BrandController : Controller
    {
        private readonly AllupDbContext _context;
        public BrandController(AllupDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.OrderByDescending(t=>t.CreatedAt).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View();
            }

            if (brand.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View();
            }

            if (await _context.Brands.AnyAsync(b => b.Name.ToLower() == brand.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View();
            }

            brand.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Brand brand)
        {
            if (!ModelState.IsValid) return View(brand);

            if (id == null) return BadRequest();

            if (id != brand.Id) return BadRequest();

            Brand dbBrand =await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (dbBrand == null) return NotFound();

            if (string.IsNullOrWhiteSpace(brand.Name))
            {
                ModelState.AddModelError("Name", "Bosluq Olmamalidir");
                return View(brand);
            }

            if (brand.Name.CheckString())
            {
                ModelState.AddModelError("Name", "Yalniz Herf Ola Biler");
                return View(brand);
            }

            if(await _context.Brands.AnyAsync(b => b.Id != id && b.Name.ToLower() == brand.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Alreade Exists");
                return View(brand);
            }

            dbBrand.Name = brand.Name;
            dbBrand.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (dbBrand == null) return NotFound();

            dbBrand.IsDeleted = true;
            dbBrand.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return PartialView("_BrandIndexPartial",await _context.Brands.OrderByDescending(b => b.CreatedAt).ToListAsync());
        }

        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest();

            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (dbBrand == null) return NotFound();

            dbBrand.IsDeleted = false;

            await _context.SaveChangesAsync();

            return PartialView("_BrandIndexPartial", await _context.Brands.OrderByDescending(b => b.CreatedAt).ToListAsync());
        }
    }
}
