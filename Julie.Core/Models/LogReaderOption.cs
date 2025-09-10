using Julie.Core.Services.LogReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Core.Models
{
    public class LogReaderOption
    {
        public string Name { get; set; } = string.Empty;
        public ILogReader Reader { get; set; } = null!;
    }
}
