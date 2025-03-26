namespace ConnectionLogger.Messaging.Messages;

public class SearchUsersByIpPartMessage
{
    public required string IpPart { get; set; }

    public required string Protocol { get; set; }
}
