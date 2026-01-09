using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;
using DierentuinApp.Services;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/zoo")]
public class ZooApiController : ControllerBase
{
    private readonly ZooContext _context;
    private readonly ZooService _zooService;

    public ZooApiController(ZooContext context, ZooService zooService)
    {
        _context = context;
        _zooService = zooService;
    }

    /// <summary>
    /// Simuleert zonsopgang: toont per verblijf welke dieren wakker worden en welke gaan slapen.
    /// </summary>
    [HttpGet("sunrise")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> Sunrise()
    {
        var result = await _zooService.GetZooSunriseAsync();
        return Ok(result);
    }

    /// <summary>
    /// Simuleert zonsondergang: toont per verblijf welke dieren wakker worden en welke gaan slapen.
    /// </summary>
    [HttpGet("sunset")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> Sunset()
    {
        var result = await _zooService.GetZooSunsetAsync();
        return Ok(result);
    }

    /// <summary>
    /// Toont per verblijf wat elk dier eet, met waarschuwingen bij gevaarlijke combinaties.
    /// </summary>
    [HttpGet("feedingtime")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> FeedingTime()
    {
        var result = await _zooService.GetZooFeedingTimeAsync();
        return Ok(result);
    }

    /// <summary>
    /// Controleert alle verblijven op problemen: ruimtegebrek, beveiliging, prooi/roofdier conflicten.
    /// </summary>
    [HttpGet("checkconstraints")]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, List<string>>>>> CheckConstraints()
    {
        var result = await _zooService.CheckZooConstraintsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Wijst automatisch dieren toe aan geschikte verblijven. Met clearExisting=true wordt eerst alles gereset.
    /// </summary>
    [HttpPost("autoassign")]
    public async Task<ActionResult<string>> AutoAssign([FromQuery] bool clearExisting = false)
    {
        var result = await _zooService.AutoAssignAsync(clearExisting);
        return Ok(result);
    }
}

