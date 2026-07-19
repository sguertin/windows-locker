using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WindowsLocker.Core.Logging;
using WindowsLocker.Core.Services;

namespace WindowsLocker.Console;

public static class ConfigFactory
{
    public static IConfiguration CreateConfiguration(string time, LogLevel logLevel = LogLevel.Information)
    {
        var settings = new Dictionary<string, string> { { "Time", time } };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings!)
            .Build();
    }
}