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

    // GET: api/zoo/sunrise - Zonsopgang voor hele dierentuin
    [HttpGet("sunrise")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> Sunrise()
    {
        var result = await _zooService.GetZooSunriseAsync();
        return Ok(result);
    }

    // GET: api/zoo/sunset - Zonsondergang voor hele dierentuin
    [HttpGet("sunset")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> Sunset()
    {
        var result = await _zooService.GetZooSunsetAsync();
        return Ok(result);
    }

    // GET: api/zoo/feedingtime - Voertijd voor hele dierentuin
    [HttpGet("feedingtime")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> FeedingTime()
    {
        var result = await _zooService.GetZooFeedingTimeAsync();
        return Ok(result);
    }

    // GET: api/zoo/checkconstraints - Check alle constraints
    [HttpGet("checkconstraints")]
    public async Task<ActionResult<Dictionary<string, Dictionary<string, List<string>>>>> CheckConstraints()
    {
        var result = await _zooService.CheckZooConstraintsAsync();
        return Ok(result);
    }

    // POST: api/zoo/autoassign?clearExisting=false - Wijs dieren automatisch toe aan verblijven
    [HttpPost("autoassign")]
    public async Task<ActionResult<string>> AutoAssign([FromQuery] bool clearExisting = false)
    {
        var result = await _zooService.AutoAssignAsync(clearExisting);
        return Ok(result);
    }
}

