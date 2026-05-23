using System.Text.Json;
using project.Models;

namespace project.Utils;

public class ConfigurationManager {
    private readonly Dictionary<string, List<FieldConfig>> _config;
    private readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public ConfigurationManager(string filePath) {
        string jsonString = File.ReadAllText(filePath);
        _config = JsonSerializer.Deserialize<Dictionary<string, List<FieldConfig>>>(jsonString, options) ?? [];
    }

    public List<FieldConfig>? GetConfigs(string name) {
        if (!_config.TryGetValue(name, out var configs)) {
            throw new KeyNotFoundException($"configuration for {name} doesn't exist");
        }

        return configs;
    }
}
