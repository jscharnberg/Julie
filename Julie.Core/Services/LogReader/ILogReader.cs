using Julie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Core.Services.LogReader
{
    public interface ILogReader
    {
        public IEnumerable<LogEntry> ReadFile(string filePath);
    }
}
