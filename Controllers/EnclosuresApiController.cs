using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Models;
using DierentuinApp.Services;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/enclosures")]
public class EnclosuresApiController : ControllerBase
{
    private readonly ZooContext _context;
    private readonly ZooService _zooService;

    public EnclosuresApiController(ZooContext context, ZooService zooService)
    {
        _context = context;
        _zooService = zooService;
    }

    /// <summary>
    /// Haalt alle verblijven op. Ondersteunt filtering op naam, klimaat en beveiligingsniveau.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enclosure>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] Climate? climate,
        [FromQuery] SecurityLevel? security)
    {
        var query = _context.Enclosures
            .Include(e => e.Animals)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(e => e.Name.Contains(search));
        if (climate.HasValue)
            query = query.Where(e => e.Climate == climate);
        if (security.HasValue)
            query = query.Where(e => e.SecurityLevel == security);

        return Ok(await query.ToListAsync());
    }

    /// <summary>
    /// Haalt een specifiek verblijf op inclusief alle dieren die erin zitten.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Enclosure>> GetById(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null)
        {
            return NotFound();
        }

        return Ok(enclosure);
    }

    /// <summary>
    /// Maakt een nieuw verblijf aan met opgegeven klimaat, habitat en beveiliging.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Enclosure>> Create(Enclosure enclosure)
    {
        _context.Enclosures.Add(enclosure);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, enclosure);
    }

    /// <summary>
    /// Wijzigt een bestaand verblijf. Dieren blijven gekoppeld aan het verblijf.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Enclosure enclosure)
    {
        if (id != enclosure.Id)
        {
            return BadRequest();
        }

        _context.Entry(enclosure).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EnclosureExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Verwijdert een verblijf. Let op: dieren in dit verblijf worden niet automatisch verwijderd.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var enclosure = await _context.Enclosures.FindAsync(id);
        if (enclosure == null)
        {
            return NotFound();
        }

        _context.Enclosures.Remove(enclosure);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EnclosureExists(int id)
    {
        return _context.Enclosures.Any(e => e.Id == id);
    }

    #region Acties

    /// <summary>
    /// Toont voor elk dier in dit verblijf wat er gebeurt bij zonsopgang.
    /// </summary>
    [HttpGet("{id}/sunrise")]
    public async Task<ActionResult<List<string>>> Sunrise(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureSunrise(enclosure));
    }

    /// <summary>
    /// Toont voor elk dier in dit verblijf wat er gebeurt bij zonsondergang.
    /// </summary>
    [HttpGet("{id}/sunset")]
    public async Task<ActionResult<List<string>>> Sunset(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureSunset(enclosure));
    }

    /// <summary>
    /// Toont voor elk dier wat het eet. Waarschuwt bij gevaarlijke prooi/roofdier combinaties.
    /// </summary>
    [HttpGet("{id}/feedingtime")]
    public async Task<ActionResult<List<string>>> FeedingTime(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureFeedingTime(enclosure));
    }

    /// <summary>
    /// Controleert ruimte, beveiliging en prooi/roofdier conflicten in dit verblijf.
    /// </summary>
    [HttpGet("{id}/checkconstraints")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> CheckConstraints(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.CheckEnclosureConstraints(enclosure));
    }

    #endregion
}
