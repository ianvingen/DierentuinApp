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

    /// <summary>
    /// Haalt alle dieren op. Ondersteunt filtering op naam/soort, categorie, verblijf, grootte en dieet.
    /// </summary>
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

    /// <summary>
    /// Haalt een specifiek dier op inclusief categorie en verblijf informatie.
    /// </summary>
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

    /// <summary>
    /// Maakt een nieuw dier aan in de dierentuin. Retourneert het aangemaakte dier met ID.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Animal>> Create(Animal animal)
    {
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = animal.Id }, animal);
    }

    /// <summary>
    /// Wijzigt een bestaand dier. Alle velden worden overschreven met de meegegeven waarden.
    /// </summary>
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

    /// <summary>
    /// Verwijdert een dier permanent uit de dierentuin.
    /// </summary>
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

    /// <summary>
    /// Geeft aan of dit dier wakker wordt, gaat slapen, of altijd actief is bij zonsopgang.
    /// </summary>
    [HttpGet("{id}/sunrise")]
    public async Task<ActionResult<string>> Sunrise(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        return Ok(_zooService.GetSunriseStatus(animal));
    }

    /// <summary>
    /// Geeft aan of dit dier wakker wordt, gaat slapen, of altijd actief is bij zonsondergang.
    /// </summary>
    [HttpGet("{id}/sunset")]
    public async Task<ActionResult<string>> Sunset(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null) return NotFound();

        return Ok(_zooService.GetSunsetStatus(animal));
    }

    /// <summary>
    /// Toont wat dit dier eet. Waarschuwt als er prooidieren in hetzelfde verblijf zitten.
    /// </summary>
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

    /// <summary>
    /// Controleert of dit dier aan alle eisen voldoet (verblijf, beveiliging). Retourneert lijst met problemen.
    /// </summary>
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
