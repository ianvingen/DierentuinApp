using Microsoft.AspNetCore.Mvc;
using DierentuinApp.Services;

namespace DierentuinApp.Controllers;

public class ZooController : Controller
{
    private readonly ZooService _zooService;

    public ZooController(ZooService zooService)
    {
        _zooService = zooService;
    }

    // GET: Zoo - Overzichtspagina met alle acties
    public IActionResult Index()
    {
        return View();
    }

    // GET: Zoo/Sunrise
    public async Task<IActionResult> Sunrise()
    {
        var result = await _zooService.GetZooSunriseAsync();
        return View(result);
    }

    // GET: Zoo/Sunset
    public async Task<IActionResult> Sunset()
    {
        var result = await _zooService.GetZooSunsetAsync();
        return View(result);
    }

    // GET: Zoo/FeedingTime
    public async Task<IActionResult> FeedingTime()
    {
        var result = await _zooService.GetZooFeedingTimeAsync();
        return View(result);
    }

    // GET: Zoo/CheckConstraints
    public async Task<IActionResult> CheckConstraints()
    {
        var result = await _zooService.CheckZooConstraintsAsync();
        return View(result);
    }

    // GET: Zoo/AutoAssign
    public IActionResult AutoAssign()
    {
        return View();
    }

    // POST: Zoo/AutoAssign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAssign(bool clearExisting)
    {
        var result = await _zooService.AutoAssignAsync(clearExisting);
        TempData["Message"] = result;
        return RedirectToAction(nameof(AutoAssign));
    }
}

