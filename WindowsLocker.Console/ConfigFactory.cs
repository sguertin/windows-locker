using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WindowsLocker.Console;

public static class ConfigFactory
{
    public static IConfiguration CreateConfiguration(string time, LogLevel logLevel = LogLevel.Information)
    {
        var settings = new Dictionary<string, string> { { "TimeLimit", time } };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
}
