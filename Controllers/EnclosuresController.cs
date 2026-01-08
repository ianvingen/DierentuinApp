using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;

namespace DierentuinApp.Controllers;

public class EnclosuresController : Controller
{
    private readonly ZooContext _context;

    public EnclosuresController(ZooContext context)
    {
        _context = context;
    }

    // GET: Enclosures
    public async Task<IActionResult> Index()
    {
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();
        return View(enclosures);
    }

    // GET: Enclosures/Details/{id}
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null)
        {
            return NotFound();
        }

        return View(enclosure);
    }

    // GET: Enclosures/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Enclosures/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Enclosure enclosure)
    {
        if (ModelState.IsValid)
        {
            _context.Add(enclosure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(enclosure);
    }

    // GET: Enclosures/Edit/{id}
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var enclosure = await _context.Enclosures.FindAsync(id);
        if (enclosure == null)
        {
            return NotFound();
        }
        return View(enclosure);
    }

    // POST: Enclosures/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Enclosure enclosure)
    {
        if (id != enclosure.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(enclosure);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnclosureExists(enclosure.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(enclosure);
    }

    // GET: Enclosures/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null)
        {
            return NotFound();
        }

        return View(enclosure);
    }

    // POST: Enclosures/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var enclosure = await _context.Enclosures.FindAsync(id);
        if (enclosure != null)
        {
            _context.Enclosures.Remove(enclosure);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool EnclosureExists(int id)
    {
        return _context.Enclosures.Any(e => e.Id == id);
    }
}

