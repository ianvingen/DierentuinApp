using Microsoft.AspNetCore.Mvc;

namespace DierentuinApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new[] { "Leeuw", "Olifant", "Giraffe" });
    }
}