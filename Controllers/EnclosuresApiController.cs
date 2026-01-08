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

    // GET: api/enclosures - Haalt alle verblijven op
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enclosure>>> GetAll()
    {
        // Include Animals zodat je kan zien welke dieren in elk verblijf zitten
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();
        return Ok(enclosures);
    }

    // GET: api/enclosures/{id} - Haalt een specifiek verblijf op
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

    // POST: api/enclosures - Maakt een nieuw verblijf aan
    [HttpPost]
    public async Task<ActionResult<Enclosure>> Create(Enclosure enclosure)
    {
        _context.Enclosures.Add(enclosure);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, enclosure);
    }

    // PUT: api/enclosures/{id} - Update een bestaand verblijf
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

    // DELETE: api/enclosures/{id} - Verwijdert een verblijf
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

    // GET: api/enclosures/{id}/sunrise - Zonsopgang voor dit verblijf
    [HttpGet("{id}/sunrise")]
    public async Task<ActionResult<List<string>>> Sunrise(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureSunrise(enclosure));
    }

    // GET: api/enclosures/{id}/sunset - Zonsondergang voor dit verblijf
    [HttpGet("{id}/sunset")]
    public async Task<ActionResult<List<string>>> Sunset(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureSunset(enclosure));
    }

    // GET: api/enclosures/{id}/feedingtime - Voedertijd voor dit verblijf
    [HttpGet("{id}/feedingtime")]
    public async Task<ActionResult<List<string>>> FeedingTime(int id)
    {
        var enclosure = await _context.Enclosures
            .Include(e => e.Animals)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enclosure == null) return NotFound();

        return Ok(_zooService.GetEnclosureFeedingTime(enclosure));
    }

    // GET: api/enclosures/{id}/checkconstraints - Check constraints voor dit verblijf
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
