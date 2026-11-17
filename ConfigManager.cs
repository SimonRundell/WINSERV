// ConfigManager.cs
// ─────────────────────────────────────────────────────────────────────────────
// Handles reading and writing config.json to/from an AppConfig object.
// Uses Newtonsoft.Json (Json.NET) — the standard JSON library for
// .NET Framework projects.
// ─────────────────────────────────────────────────────────────────────────────

using Newtonsoft.Json;

namespace WINSERV;

public static class ConfigManager
{
    private static readonly string ConfigFilePath = Path.Combine(
        AppContext.BaseDirectory,
        "config.json"
    );

    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented   // Human-readable JSON with newlines + spaces
    };

    /// <summary>Returns the file system path where config.json is stored.</summary>
    public static string GetConfigFilePath() => ConfigFilePath;

    /// <summary>
    /// Loads AppConfig from config.json.
    /// Returns defaults on first run or if the file is corrupt.
    /// </summary>
    public static AppConfig Load()
    {
        if (!File.Exists(ConfigFilePath))
            return new AppConfig();

        try
        {
            string json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
        }
        catch (JsonException ex)
        {
            MessageBox.Show(
                $"config.json appears to be corrupt and could not be read.\n\n" +
                $"Error: {ex.Message}\n\n" +
                $"WINSERV will use default settings. You can reconfigure via Settings.",
                "WINSERV — Config Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return new AppConfig();
        }
        catch (IOException ex)
        {
            MessageBox.Show(
                $"Could not read config.json:\n\n{ex.Message}\n\nUsing defaults.",
                "WINSERV — Config Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return new AppConfig();
        }
    }

    /// <summary>Saves the given AppConfig to config.json.</summary>
    public static void Save(AppConfig config)
    {
        string json = JsonConvert.SerializeObject(config, JsonSettings);
        File.WriteAllText(ConfigFilePath, json);
    }
}
