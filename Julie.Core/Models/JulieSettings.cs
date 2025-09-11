using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Core.Models
{
    public class JulieSettings
    {
        public string LoggerType { get; set; } = "Serilog"; // oder "Cotas"
        public string Template { get; set; } = @"^(?<Timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [+-]\d{2}:\d{2}) " +
            @"\[(?<Level>[A-Z]{3})\] " +
            @"\((?<SourceContext>.*?)\) " +           // SourceContext kann leer sein
            @"\((?<Method>.*?):?(?<Line>\d*)\) " +  // Method und Line optional
            @"(?<Message>.*)$";
        public List<string> AvailableLoggers { get; set; } = new() { "Cotas", "Serilog" };
    }
}
