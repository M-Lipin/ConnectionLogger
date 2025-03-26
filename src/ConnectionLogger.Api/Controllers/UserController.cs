using ConnectionLogger.Data.Services;
using ConnectionLogger.Messaging.Messages;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsersByIpPart([FromQuery] string ipPart, [FromQuery] string protocol)
    {
        var message = new SearchUsersByIpPartMessage()
        {
            IpPart = ipPart,
            Protocol = protocol
        };

        var result = await _userService.SearchUsersByIpPartAsync(message.IpPart, message.Protocol);

        return Ok(result);
    }

    [HttpGet("{userId}/ips")]
    public async Task<IActionResult> GetUserIps(long userId)
    {
        var message = new GetUserIpsMessage()
        {
            UserId = userId
        };

        var result = await _userService.GetUserIpsAsync(userId);

        return Ok(result);
    }
}
