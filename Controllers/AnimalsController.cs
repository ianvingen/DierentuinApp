using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;

namespace DierentuinApp.Controllers;

public class AnimalsController : Controller
{
    private readonly ZooContext _context;

    public AnimalsController(ZooContext context)
    {
        _context = context;
    }

    // GET: Animals
    public async Task<IActionResult> Index()
    {
        var animals = await _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .ToListAsync();
        return View(animals);
    }

    // GET: Animals/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    // GET: Animals/Create
    public IActionResult Create()
    {
        LoadDropdowns();
        return View();
    }

    // POST: Animals/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Animal animal)
    {
        if (ModelState.IsValid)
        {
            _context.Add(animal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(animal);
        return View(animal);
    }

    // GET: Animals/Edit/{ID}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }
        LoadDropdowns(animal);
        return View(animal);
    }

    // POST: Animals/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Animal animal)
    {
        if (id != animal.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(animal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(animal.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        LoadDropdowns(animal);
        return View(animal);
    }

    // GET: Animals/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var animal = await _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null)
        {
            return NotFound();
        }

        return View(animal);
    }

    // POST: Animals/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal != null)
        {
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Check of animal bestaat
    private bool AnimalExists(int id)
    {
        return _context.Animals.Any(e => e.Id == id);
    }

    // Vul dropdowns voor Category en Enclosure
    private void LoadDropdowns(Animal? animal = null)
    {
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", animal?.CategoryId);
        ViewBag.Enclosures = new SelectList(_context.Enclosures, "Id", "Name", animal?.EnclosureId);
    }
}

