using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;
using DierentuinApp.Services;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/animals")]
public class AnimalsApiController : ControllerBase
{
    private readonly ZooContext _context;
    private readonly ZooService _zooService;

    public AnimalsApiController(ZooContext context, ZooService zooService)
    {
        _context = context;
        _zooService = zooService;
    }

    // GET: api/animals - Haalt alle dieren op met optionele filters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? enclosureId,
        [FromQuery] AnimalSize? size,
        [FromQuery] DietaryClass? diet)
    {
        var query = _context.Animals
            .Include(a => a.Category)
            .Include(a => a.Enclosure)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Name.Contains(search) || a.Species.Contains(search));
        if (categoryId.HasValue)
            query = query.Where(a => a.CategoryId == categoryId);
        if (enclosureId.HasValue)
            query = query.Where(a => a.EnclosureId == enclosureId);
        if (size.HasValue)
            query = query.Where(a => a.Size == size);
        if (diet.HasValue)
            query = query.Where(a => a.DietaryClass == diet);

        return Ok(await query.ToListAsync());
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

    #region Acties

    // GET: api/animals/{id}/sunrise - Wat doet dit dier bij zonsopgang?
    [HttpGet("{id}/sunrise")]
    public async Task<ActionResult<string>> Sunrise(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        return Ok(_zooService.GetSunriseStatus(animal));
    }

    // GET: api/animals/{id}/sunset - Wat doet dit dier bij zonsondergang?
    [HttpGet("{id}/sunset")]
    public async Task<ActionResult<string>> Sunset(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        return Ok(_zooService.GetSunsetStatus(animal));
    }

    // GET: api/animals/{id}/feedingtime - Wat eet dit dier?
    [HttpGet("{id}/feedingtime")]
    public async Task<ActionResult<string>> FeedingTime(int id)
    {
        var animal = await _context.Animals
            .Include(a => a.Enclosure)
                .ThenInclude(e => e!.Animals)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null) return NotFound();

        var enclosureAnimals = animal.Enclosure?.Animals;
        return Ok(_zooService.GetFeedingInfo(animal, enclosureAnimals));
    }

    // GET: api/animals/{id}/checkconstraints - Voldoet dit dier aan alle eisen?
    [HttpGet("{id}/checkconstraints")]
    public async Task<ActionResult<List<string>>> CheckConstraints(int id)
    {
        var animal = await _context.Animals
            .Include(a => a.Enclosure)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (animal == null) return NotFound();

        var issues = _zooService.CheckAnimalConstraints(animal);
        if (!issues.Any())
        {
            issues.Add("Alle constraints voldaan ✓");
        }
        return Ok(issues);
    }

    #endregion
}
