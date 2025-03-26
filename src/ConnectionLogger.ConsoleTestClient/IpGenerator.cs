using System.Text;

namespace ConnectionLogger.ConsoleTestClient;

public static class IpGenerator
{
    private static readonly Random random = new Random();
    private const string hexChars = "0123456789abcdef";

    public static string GenerateIPv4()
    {
        return $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";
    }

    public static string GenerateIPv6()
    {
        var ipv6 = new StringBuilder();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                ipv6.Append(hexChars[random.Next(hexChars.Length)]);
            }

            if (i < 7)
            {
                ipv6.Append(':');
            }
        }

        return ipv6.ToString();
    }
}
