using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Converter
{
    public class ResourceConverter : IValueConverter
    {
        private static readonly System.Resources.ResourceManager _rm =
        new System.Resources.ResourceManager("Julie.Strings", typeof(ResourceConverter).Assembly);

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string key)
            {
                return _rm.GetString(key, culture) ?? key;
            }
            return value ?? "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
