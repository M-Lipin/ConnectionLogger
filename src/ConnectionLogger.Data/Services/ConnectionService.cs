using ConnectionLogger.Data.Models;
using ConnectionLogger.Messaging.Messages;
using Microsoft.EntityFrameworkCore;

namespace ConnectionLogger.Data.Services;

public class ConnectionService : IConnectionService
{
    private readonly AppDbContext _dbContext;

    public ConnectionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Connection> SaveConnectionAsync(long userId, string address, string protocol)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var ipAddress = await _dbContext.IpAddresses
                .FirstOrDefaultAsync(ip => ip.Address == address && ip.Protocol == protocol);

            if (ipAddress == null)
            {
                ipAddress = new IpAddress { Address = address, Protocol = protocol };
                await _dbContext.IpAddresses.AddAsync(ipAddress);
                await _dbContext.SaveChangesAsync();
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                // TODO: Consider passing user's FirstName and LastName.
                user = new User { Id = userId, FirstName = "Ivan", LastName = "Ivanov" };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }

            DateTime now = DateTime.UtcNow;
            var existingConnection = await _dbContext.Connections.FirstOrDefaultAsync(c =>
                c.UserId == user.Id &&
                c.IpAddressId == ipAddress.Id &&
                c.ConnectedAt.Date == now.Date &&
                c.ConnectedAt.Hour == now.Hour &&
                c.ConnectedAt.Minute == now.Minute &&
                c.ConnectedAt.Second == now.Second);

            if (existingConnection != null)
            {
                return existingConnection;
            }

            var connection = new Connection
            {
                User = user,
                IpAddress = ipAddress,
                ConnectedAt = now
            };

            await _dbContext.Connections.AddAsync(connection);
            await _dbContext.SaveChangesAsync();

            return connection;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new ApplicationException("Connection saving failed.", ex);
        }
    }

    public async Task<List<Connection>> GetConnectionsAsync(long userId, OrderBy orderBy, Direction direction)
    {
        var query = _dbContext.Connections.AsQueryable();

        query = query.Where(c => c.UserId == userId);

        if (orderBy == OrderBy.DateCreated)
        {
            query = direction == Direction.Asc
                ? query.OrderBy(c => c.ConnectedAt)
                : query.OrderByDescending(c => c.ConnectedAt);
        }
        else if (orderBy == OrderBy.IpAddress)
        {
            query = direction == Direction.Asc
                ? query.OrderBy(c => c.IpAddressId)
                : query.OrderByDescending(c => c.IpAddressId);
        }
        else
        {
            throw new NotSupportedException($"Ordering by {orderBy} is not supported.");
        }

        return await query.ToListAsync();
    }
}
