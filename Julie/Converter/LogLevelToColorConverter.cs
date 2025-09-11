using Julie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Data.Converters;
using Serilog;

namespace Julie.Converter
{
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                return level == LogLevel.Error ? "#e6721d" : "#1b7784";
            }

            return "#1b7784";

            //if (value is LogLevel level)
            //{
            //    return level switch
            //    {
            //        LogLevel.Warning => "#FFC107",
            //        LogLevel.Error => "#F44336",
            //        _ => "#FFFFFF"
            //    };
            //}
            //return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
