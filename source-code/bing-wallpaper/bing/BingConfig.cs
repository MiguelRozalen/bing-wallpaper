using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bing_wallpaper
{
    public class BingConfig
    {
        public bool runAtStartup { get; set; }
        public bool setWallpaper { get; set; }
        public bool setLockScreen { get; set; }
        public string period { get; set; }
        public CultureInfo languageOptions { get; set; }

        public BingConfig() { }
    }
}
