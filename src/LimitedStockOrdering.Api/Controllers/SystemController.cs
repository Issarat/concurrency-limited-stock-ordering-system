using Microsoft.AspNetCore.Mvc;

namespace LimitedStockOrdering.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType<SystemStatusResponse>(StatusCodes.Status200OK)]
    public ActionResult<SystemStatusResponse> GetStatus()
    {
        return Ok(new SystemStatusResponse(
            "LimitedStockOrdering.Api",
            "ok",
            DateTimeOffset.UtcNow));
    }
}

public sealed record SystemStatusResponse(
    string Service,
    string Status,
    DateTimeOffset UtcTime);
