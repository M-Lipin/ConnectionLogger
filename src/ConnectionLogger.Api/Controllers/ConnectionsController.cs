using ConnectionLogger.Data.Services;
using ConnectionLogger.Messaging.Messages;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/connections")]
public class ConnectionsController : ControllerBase
{
    private readonly IConnectionService _dataService;

    public ConnectionsController(IConnectionService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public async Task<IActionResult> GetConnections([FromQuery] long userId, [FromQuery] string orderBy = "DateCreated", [FromQuery] string direction = "Desc")
    {
        if (!Enum.TryParse(orderBy, true, out OrderBy orderByValue))
        {
            return BadRequest(new { message = $"orderBy must equal one of the following strings: {string.Join(", ", Enum.GetNames(typeof(OrderBy)))}" });
        }

        if (!Enum.TryParse(direction, true, out Direction directionValue))
        {
            return BadRequest(new { message = $"direction must equal one of the following strings: {string.Join(", ", Enum.GetNames(typeof(Direction)))}" });
        }

        var result = await _dataService.GetConnectionsAsync(userId, orderByValue, directionValue);

        return Ok(result);
    }
}
