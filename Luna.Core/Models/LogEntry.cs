using Luna.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Luna.Core.Models
{
    public class LogEntry
    {
        public int Line { get; set; }   
        public LogLevel LogType { get; set; }
        public DateTime Stamp { get; set; }
        public string File { get; set; }
        public int LogLine { get; set; }
        public string Thread { get; set; }
        public string Method { get; set; }
        public string Text { get; set; }
    }
}
