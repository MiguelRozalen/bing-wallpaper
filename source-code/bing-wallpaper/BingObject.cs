using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bing_wallpaper
{
    public class BingObject
    {
        public IList<BingImage> images { get; set; }
        public BingTooltip tooltips { get; set; }

        public BingObject() { }
    }
}
