using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bing_wallpaper_service_manager
{
    public class BingWallpaperServiceManager
    {
        static void Main(string[] args)
        {
            /*args = new string[4];
            args[0] = "BingWallpaperTesting";
            args[1] = "Esto es una prueba";
            args[2] = "True";
            args[3] = "Every Day";*/
            if (args.Length == 4)
            {
                UtilsScheduleTask.CreateScheduleTask(args[0], args[1], args[2], args[3]);
            }
            if (args.Length == 1)
            {
                UtilsScheduleTask.DeleteScheduleTask(args[0]);
            }
        }
    }
}
