using ConnectionLogger.Data.Models;

namespace ConnectionLogger.Data.Services;

public interface IUserService
{
    Task<List<long>> SearchUsersByIpPartAsync(string ipPart, string protocol);

    Task<List<string>> GetUserIpsAsync(long userId);
}
