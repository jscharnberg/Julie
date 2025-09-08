using Luna.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luna.Core.Services
{
    public class LogReader
    {
        public IEnumerable<LogEntry> ReadFile(string path)
        {
            foreach (var line in File.ReadLines(path))
            {
                yield return new LogEntry
                {
                    Timestamp = DateTime.Now,   // TODO: später parsen
                    Level = "Info",
                    Message = line,
                    Source = Path.GetFileName(path)
                };
            }
        }
    }
}
