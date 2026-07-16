using static System.Text.Json.JsonSerializer;
using static LockerService.Constants;
namespace LockerService;

public class ConfigurationService(string configFilePath)
{
    private readonly LogService _log = new (APPLICATION_NAME);
    private readonly AppConfig _defaultConfiguration = new ()
    {
        Time = "5:30 PM"
    };
    public async Task<AppConfig> GetConfig(CancellationToken stoppingToken = default)
    {
        AppConfig? config = null;
        if (File.Exists(configFilePath))
        {
            var content = await File.ReadAllTextAsync(configFilePath, stoppingToken);
            config = Deserialize<AppConfig>(content);
        }
        if (config != null) return config;
        
        config = _defaultConfiguration;
        _log.LogInformation($"Could not find config file at {configFilePath}, using default configuration.");
        await File.WriteAllTextAsync(configFilePath, Serialize(config), stoppingToken);
        return config;
    }
}