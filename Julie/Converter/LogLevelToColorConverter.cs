using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Julie.Core.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Converter
{
    public class LogLevelToColorConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value is LogLevel level)
        //    {
        //        return level == LogLevel.Error ? "#e6721d" : "#1b7784";
        //    }

        //    return "#1b7784";

        //    //if (value is LogLevel level)
        //    //{
        //    //    return level switch
        //    //    {
        //    //        LogLevel.Warning => "#FFC107",
        //    //        LogLevel.Error => "#F44336",
        //    //        _ => "#FFFFFF"
        //    //    };
        //    //}
        //    //return "#FFFFFF";
        //}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel level)
            {
                object resource = null;

                switch (level)
                {
                    case LogLevel.Error:
                        Application.Current?.TryFindResource("ErrorBrush", out resource);
                        break;
                    case LogLevel.Warning:
                        Application.Current?.TryFindResource("WarningBrush", out resource);
                        break;
                    case LogLevel.Debug:
                        Application.Current?.TryFindResource("InfoBrush", out resource);
                        break;
                    case LogLevel.Sequence: // Info
                        Application.Current?.TryFindResource("InfoBrush", out resource);
                        break;
                }

                return resource ?? Brushes.Gray;
            }

            return Brushes.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
