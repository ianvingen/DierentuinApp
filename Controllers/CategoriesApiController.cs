using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesApiController : ControllerBase
{
    private readonly ZooContext _context;

    public CategoriesApiController(ZooContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Haalt alle categorieën op (bijv. Zoogdieren, Vogels). Ondersteunt zoeken op naam.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll([FromQuery] string? search)
    {
        var query = _context.Categories
            .Include(c => c.Animals)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(c => c.Name.Contains(search));

        return Ok(await query.ToListAsync());
    }

    /// <summary>
    /// Haalt een specifieke categorie op inclusief alle dieren in die categorie.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Animals)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    /// <summary>
    /// Maakt een nieuwe categorie aan voor het groeperen van dieren.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Wijzigt de naam van een bestaande categorie.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Verwijdert een categorie. Dieren in deze categorie worden niet verwijderd.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
