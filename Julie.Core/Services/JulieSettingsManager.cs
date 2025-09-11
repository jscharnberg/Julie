using Julie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Julie.Core.Services
{
    public static class JulieSettingsManager
    {
        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Julie", "settings.json");

        public static JulieSettings Load()
        {
            if (!File.Exists(SettingsFile))
            {
                var defaultSettings = new JulieSettings
                {
                    LoggerType = "Serilog",
                    Template = @"^(?<Timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2}) \[(?<Level>[A-Z]{3})\] \((?<SourceContext>.*?)\) \((?<Method>.*?)\) (?<Message>.*)$"
                };

                // Datei-Ordner sicherstellen
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);

                // Datei speichern
                File.WriteAllText(SettingsFile, System.Text.Json.JsonSerializer.Serialize(defaultSettings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

                return defaultSettings;
            }

            try
            {
                var json = File.ReadAllText(SettingsFile);
                return System.Text.Json.JsonSerializer.Deserialize<JulieSettings>(json)
                       ?? throw new Exception("Settings konnten nicht geladen werden.");
            }
            catch
            {
                // Fallback: Default zurückgeben, falls Datei beschädigt
                return new JulieSettings
                {
                    LoggerType = "Serilog",
                    Template = @"^(?<Timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2}) \[(?<Level>[A-Z]{3})\] \((?<SourceContext>.*?)\) \((?<Method>.*?)\) (?<Message>.*)$"
                };
            }
        }

        public static void Save(JulieSettings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile)!);
            File.WriteAllText(SettingsFile, System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        }
    }

}
