using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Julie.Core.Models
{
    public static class Location
    {
        private static readonly ResourceManager _rm = new ResourceManager("Julie.Resources.Strings", typeof(Location).Assembly);

        public static string Get(string key) => _rm.GetString(key, CultureInfo.CurrentUICulture) ?? key;

        public static void SetCulture(string cultureCode)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(cultureCode);
            CultureInfo.CurrentCulture = new CultureInfo(cultureCode);
        }
    }
}
