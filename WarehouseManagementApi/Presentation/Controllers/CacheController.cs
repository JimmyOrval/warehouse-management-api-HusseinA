using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/cache")]
public class CacheController(ICacheStatsTracker cacheStats) : ControllerBase
{
    [HttpGet("stats")]
    public IActionResult GetStats() => Ok(cacheStats.GetStats());
}