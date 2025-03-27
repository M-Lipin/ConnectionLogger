using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ConnectionLogger.ConsoleTestClient;

public class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static string host = "http://localhost:";

    static async Task Main()
    {
        Console.Write("Specify port (https://localhost:<port>) :");
        int port = int.Parse(Console.ReadLine() ?? "7007");
        host += port;

        Console.Write("Specify the number of requests: ");
        int requestCount = int.Parse(Console.ReadLine() ?? "10");

        // Console.Write("Specify userId min value: ");
        // int userIdStart = int.Parse(Console.ReadLine() ?? "1000");
        int userIdStart = 1000;

        // Console.Write("Specify userId max value: ");
        // int userIdFinish = int.Parse(Console.ReadLine() ?? "10000");
        int userIdFinish = 10000;

        Console.Write("Specify the delay between requests (in milliseconds): ");
        int delayMs = int.Parse(Console.ReadLine() ?? "0");

        // Console.Write("Specify the maximum concurrency: ");
        // int maxConcurrency = int.Parse(Console.ReadLine() ?? "100");
        int maxConcurrency = 10;

        Console.WriteLine("Starting test...");

        var results = new List<string>();

        var stopwatch = Stopwatch.StartNew();

        await RunTestAsync(requestCount, userIdStart, userIdFinish, delayMs, maxConcurrency, results);

        stopwatch.Stop();

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine($"All requests have been sent! Execution time: {stopwatch.Elapsed.TotalSeconds:F2} seconds.");

        Console.WriteLine("Press any key to exit.");

        Console.ReadKey();
    }

    static async Task RunTestAsync(int requestCount, int userIdStart, int userIdFinish, int delayMs, int maxConcurrency, List<string> results)
    {
        var semaphore = new SemaphoreSlim(maxConcurrency);
        var tasks = new List<Task>();

        for (int i = 0; i < requestCount; i++)
        {
            tasks.Add(SendRequestAsync(userIdStart, userIdFinish, delayMs, semaphore, results));
        }

        await Task.WhenAll(tasks);
    }

    static async Task SendRequestAsync(int userIdStart, int userIdFinish, int delayMs, SemaphoreSlim semaphore, List<string> results)
    {
        await semaphore.WaitAsync();
        long userId = Random.Shared.Next(userIdStart, userIdFinish);
        try
        {
            await Task.Delay(delayMs);
            int typeProtocol = Random.Shared.Next(2) == 0 ? 4 : 6;
            string ip = typeProtocol == 4 ? IpGenerator.GenerateIPv4() : IpGenerator.GenerateIPv6();

            var request = new { Ip = ip };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{host}/api/users/{userId}/connect", content);
            string responseBody = await response.Content.ReadAsStringAsync();

            results.Add($"The response for UserId {userId}: Status {response.StatusCode}, Response body: {responseBody}");
        }
        catch (Exception ex)
        {
            results.Add($"An error when sending request for UserId {userId}: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}
