using System.IO;
using System.Text.Json;
using FloatingB.Core.Interfaces;
using FloatingB.Core.Models;

namespace FloatingB.Infrastructure.Settings
{
    /// <summary>
    /// Stores settings in %AppData%\FloatDesk\settings.json.
    /// </summary>
    public class JsonSettingsService : ISettingsService
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        private readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FloatDesk",
            "settings.json");

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_path))
                {
                    return new AppSettings();
                }

                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path), Options)
                    ?? new AppSettings();
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                // A missing or broken file just means defaults.
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(settings, Options));
        }
    }
}
