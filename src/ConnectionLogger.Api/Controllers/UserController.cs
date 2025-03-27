using ConnectionLogger.Data.Services;
using ConnectionLogger.Messaging.Messages;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private const string IPv4 = "IPv4";
    private const string IPv6 = "IPv6";

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsersByIpPart([FromQuery] string ipPart, [FromQuery] string ipVersion)
    {
        ipPart = ipPart.Trim();
        if (string.IsNullOrEmpty(ipPart))
        {
            return BadRequest("ipPart must not be empty");
        }

        if (!IsValidIpVersion(ipVersion))
        {
            return BadRequest("Invalid ipVersion type");
        }

        ipVersion = NormalizeIpVersion(ipVersion);

        if (!IsValidIpStart(ipPart, ipVersion))
        {
            return BadRequest("Invalid ipPart or ipPart does not match the protocol type");
        }

        var message = new SearchUsersByIpPartMessage()
        {
            IpPart = ipPart,
            Protocol = ipVersion
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

    private bool IsValidIpVersion(string ipVersion)
    {
        return Regex.IsMatch(ipVersion, @"^ipv[46]$", RegexOptions.IgnoreCase);
    }

    private string NormalizeIpVersion(string ipVersion)
    {
        string lower = ipVersion.ToLower().Trim();
        return lower switch
        {
            "ipv4" => "IPv4",
            "ipv6" => "IPv6",
            _ => ""
        };
    }

    private bool IsValidIpStart(string segment, string protocol)
    {
        if (protocol.Equals("IPv4", StringComparison.OrdinalIgnoreCase))
        {
            return IsValidIPv4Start(segment);
        }
        else if (protocol.Equals("IPv6", StringComparison.OrdinalIgnoreCase))
        {
            return IsValidIPv6Start(segment);
        }

        return false;
    }

    private bool IsValidIPv4Start(string segment)
    {
        if (Regex.IsMatch(segment, @"^\d{1,3}(\.\d{1,3}){0,2}$"))
        {
            string[] parts = segment.Split('.');
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                    return false;
            }
            return true;
        }

        return false;
    }

    private bool IsValidIPv6Start(string segment)
    {
        if (
            segment == "::" || 
            segment.StartsWith("::") || 
            Regex.IsMatch(segment, @"^[0-9a-fA-F]{1,4}$") || 
            Regex.IsMatch(segment, @"^([0-9a-fA-F]{1,4}:)+[0-9a-fA-F]{0,4}$"))
        {
            return true;
        }
        return false;
    }
}
