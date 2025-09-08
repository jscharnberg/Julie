using Luna.Core.Enums;
using Luna.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Luna.Core.Services
{
    public class LogReader
    {
        public IEnumerable<LogEntry> ReadFile(string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Beispiel: @6  08.09 10:35:20,692, Logger.cs, 28, 00000001, Logger::DoLog: erstes log

                // 1. Template und Rest splitten
                var firstSpace = line.IndexOf(' ');
                if (firstSpace < 0)
                    continue;

                var templateRaw = line.Substring(0, firstSpace).Trim();
                // @ entfernen, falls vorhanden
                var templateClean = templateRaw.StartsWith("@") ? templateRaw.Substring(1) : templateRaw;

                var rest = line.Substring(firstSpace).Trim();

                // 2. Rest nach Kommas splitten
                var parts = rest.Split(',', StringSplitOptions.TrimEntries);
                if (parts.Length < 5)
                    continue; // weil wir template schon separat haben

                if (!int.TryParse(templateClean, out int templateIndex))
                    continue;

                LogLevel logLevel = IndexToLogLevel(templateIndex);

                // 3. Methode und Text trennen
                string method = string.Empty;
                string text = string.Empty;

                var methodAndText = parts[5]; // z.B. "CotasConnection::Login: C:\app\client\..."
                int doubleColonIndex = methodAndText.IndexOf("::");

                if (doubleColonIndex >= 0)
                {
                    // Suche ersten Doppelpunkt nach dem "::"
                    int colonAfterMethod = methodAndText.IndexOf(':', doubleColonIndex + 2);
                    if (colonAfterMethod >= 0)
                    {
                        method = methodAndText.Substring(0, colonAfterMethod).Trim();  // "CotasConnection::Login"
                        text = methodAndText.Substring(colonAfterMethod + 1).Trim();   // "C:\app\client\..."
                    }
                    else
                    {
                        method = methodAndText.Trim();
                        text = string.Empty;
                    }
                }
                else
                {
                    // Falls kein "::" vorhanden
                    int firstColon = methodAndText.IndexOf(':');
                    if (firstColon >= 0)
                    {
                        method = methodAndText.Substring(0, firstColon).Trim();
                        text = methodAndText.Substring(firstColon + 1).Trim();
                    }
                    else
                    {
                        method = methodAndText.Trim();
                        text = string.Empty;
                    }
                }


                yield return new LogEntry
                {
                    LogType = logLevel,
                    Stamp = DateTime.TryParseExact(
                        parts[0],
                        "dd.MM HH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var dt) ? dt : DateTime.MinValue,
                    File = parts[2],
                    LogLine = int.TryParse(parts[3], out var lineNr) ? lineNr : 0,
                    Thread = parts[4],
                    Method = method,
                    Text = text
                };
            }
        }

        private static LogLevel IndexToLogLevel(int index)
        {
            var levels = Enum.GetValues<LogLevel>().Where(l => l != LogLevel.Null).ToArray();
            return (index >= 0 && index < levels.Length) ? levels[index] : LogLevel.Null;
        }

        private static int LogTypeToIndex(LogLevel logType)
        {
            int lt = (int)logType;
            int i = 1;

            if (lt == 0)
            {
                return -1;
            }
            if (lt == 1)
            {
                return 0;
            }

            while ((lt /= 2) > 1)
            {
                i++;
            }

            var levels = Enum.GetValues<LogLevel>();
            var maxLevel = (int)levels[^1]; //letzter eintrag

            if (i >= maxLevel)
            {
                i = maxLevel - 1;
            }
            return i;
        }
    }
}
