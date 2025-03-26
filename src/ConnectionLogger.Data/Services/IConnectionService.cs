using ConnectionLogger.Data.Models;
using ConnectionLogger.Messaging.Messages;

namespace ConnectionLogger.Data.Services;

public interface IConnectionService
{
    Task<Connection> SaveConnectionAsync(long userId, string address, string protocol);

    Task<List<Connection>> GetConnectionsAsync(long userId, OrderBy orderBy, Direction direction);
}
