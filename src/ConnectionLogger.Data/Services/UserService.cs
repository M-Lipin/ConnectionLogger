using Microsoft.EntityFrameworkCore;

namespace ConnectionLogger.Data.Services;

public class UserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<long>> GetUsersByIpAsync(string ipPart, string protocol)
    {
        return await _dbContext.Connections
            .OrderBy(c => c.IpAddress.Protocol)
            .Where(c => c.IpAddress.Address.Contains(ipPart) && c.IpAddress.Protocol == protocol)
            .Select(c => c.UserId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<string>> GetUserIpsAsync(long userId)
    {
        return await _dbContext.Connections
            .Where(c => c.UserId == userId)
            .Select(c => c.IpAddress.Address)
            .ToListAsync();
    }
}
