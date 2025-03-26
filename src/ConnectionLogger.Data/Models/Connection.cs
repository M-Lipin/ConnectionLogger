namespace ConnectionLogger.Data.Models;

public record Connection
{
    public long UserId { get; init; }

    public required User User { get; init; }

    public long IpAddressId { get; init; }

    public required IpAddress IpAddress { get; init; }

    public DateTime ConnectedAt { get; init; } = DateTime.Now;
}
