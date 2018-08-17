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
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (options.Delete)
                {
                    UtilsScheduleTask.DeleteScheduleTask(options.Name.Replace('%', ' '));
                }
                else
                {
                    UtilsScheduleTask.CreateScheduleTask(options.Name.Replace('%',' '), 
                        options.Description.Replace('%', ' '), options.RunAtStartup, options.Period.Replace('%', ' '));
                }
            }
            else
            {
                options.GetUsage();
            }
        }
    }
}
