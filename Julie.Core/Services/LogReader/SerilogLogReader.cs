using Julie.Core.Enums;
using Julie.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Julie.Core.Services.LogReader
{
    public class SerilogLogReader : ILogReader
    {
        private readonly Regex _logRegex;

        public SerilogLogReader(string templatePattern, RegexOptions regexOption)
        {
            _logRegex = new Regex(templatePattern, regexOption);
        }

        public IEnumerable<LogEntry> ReadFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);

            string? line;
            int lineNr = 1;

            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = _logRegex.Match(line);
                if (!match.Success)
                    continue;

                // Timestamp
                DateTime timestamp = DateTime.TryParseExact(
                    match.Groups["Timestamp"].Value,
                    "yyyy-MM-dd HH:mm:ss.fff zzz",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var dt) ? dt : DateTime.MinValue;

                // LogLevel
                var levelString = match.Groups["Level"].Value;
                LogLevel logLevel = levelString switch
                {
                    "INF" => LogLevel.Sequence,
                    "WRN" => LogLevel.Warning,
                    "ERR" => LogLevel.Error,
                    "DBG" => LogLevel.Debug,
                    _ => LogLevel.Null
                };

                // FileName
                string fullSource = match.Groups["SourceContext"].Value;
                string fileName = fullSource.Contains('.')
                    ? fullSource.Substring(fullSource.LastIndexOf('.') + 1)
                    : fullSource;
                
                string method = match.Groups["Method"].Success ? match.Groups["Method"].Value : string.Empty;

                int? logLine = int.TryParse(match.Groups["Line"].Value, out var ln) ? ln : null;

                // Text
                string text = match.Groups["Message"].Value;

                yield return new LogEntry
                {
                    Line = lineNr++,
                    SourceFileName = Path.GetFileName(filePath),
                    FileName = fileName,
                    LogLine = logLine,
                    Thread = string.Empty,
                    Method = method,
                    Stamp = timestamp,
                    LogType = logLevel,
                    Text = text
                };
            }
        }
    }
}
