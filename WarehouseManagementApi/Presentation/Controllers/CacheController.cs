using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/cache")]
public class CacheController(ICacheStatsTracker cacheStats) : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats() => Ok(cacheStats.GetStats());
}