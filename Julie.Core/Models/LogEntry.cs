using Julie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Core.Models
{
    public class LogEntry
    {
        public int Line { get; set; }   
        public string SourceFileName { get; set; }
        public LogLevel LogType { get; set; }
        public DateTime Stamp { get; set; }
        public string FileName { get; set; }
        public int? LogLine { get; set; }
        public string Thread { get; set; }
        public string Method { get; set; }
        public string Text { get; set; }

        public string SourceFileAndLine => $"{SourceFileName}({Line})";
        public string LogFileAndLine => $"{FileName}: {LogLine}";
    }
}
