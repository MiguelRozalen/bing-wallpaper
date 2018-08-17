using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bing_wallpaper_service_manager
{
    public class Options
    {
        [Option('n', "name", Required = false, DefaultValue = "Bing Wallpaper", HelpText = "Name of the scheduled task")]
        public string Name { get; set; }

        [Option('d', "description", Required = false, DefaultValue = "Bing Wallpaper Scheduled Task",  HelpText = "Description of the scheduled task")]
        public string Description { get; set; }

        [Option('r', "run at startup", Required = false, DefaultValue = true, HelpText = "Is it executed at startup?")]
        public bool RunAtStartup { get; set; }

        [Option('p', "period", Required = false, DefaultValue = "Every Day", HelpText = "Execution Period")]
        public string Period { get; set; }

        [Option('c', "delete", Required = false, DefaultValue = false, HelpText = "Delete task?")]
        public bool Delete { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
