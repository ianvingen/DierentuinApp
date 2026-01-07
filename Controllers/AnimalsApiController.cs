using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/animals")]
public class AnimalsApiController : ControllerBase
{
    private readonly ZooContext _context;

    public AnimalsApiController(ZooContext context)
    {
        _context = context;
    }

    // GET: api/animals - Haalt alle dieren op
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAll()
    {
        // Laad de bijbehorende Category en Enclosure mee zodat die navigation properties niet null zijn
        var animals = await _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .ToListAsync();
        return Ok(animals);
    }

    // GET: api/animals/{id} - Haalt een specifiek dier op
    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetById(int id)
    {
        var animal = await _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null)
        {
            return NotFound();
        }

        return Ok(animal);
    }

    // POST: api/animals - Maakt een nieuw dier aan
    [HttpPost]
    public async Task<ActionResult<Animal>> Create(Animal animal)
    {
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    // PUT: api/animals/{id} - Update een bestaand dier
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Animal animal)
    {
        // Komen de ids overeen?
        if (id != animal.Id)
        {
            return BadRequest();
        }

        // Markeer het hele object als gewijzigd zodat EF weet dat het moet updaten
        _context.Entry(animal).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimalExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/animals/{id} - Verwijdert een dier
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }

        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Checkt of een dier bestaat
    private bool AnimalExists(int id)
    {
        return _context.Animals.Any(e => e.Id == id);
    }
}
