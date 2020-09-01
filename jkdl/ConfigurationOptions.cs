using CommandLine;

namespace jkdl
{
    internal class ConfigurationOptions : IConfigurationOptions
    {
        [Option('b', "background", Required = false, HelpText = "Start in background mode.", Default = false)]
        public bool Background { get; set; }
        [Option('o', "overwride", Required = false, HelpText = "Overwrite file in same name on download location.", Default = false)]
        public bool OverwriteResults { get; set; }
        [Option('m', "max", Required = false, HelpText = "Max number of concurrent download.", Default = 3)]
        public int MaxNumberOfDownload { get; set; }
        [Option('l', "location", Required = false, HelpText = "Download location.", Default = ".")]
        public string DownloadLocation { get; set; }
        [Option('t', "trash", Required = false, HelpText = "Download thrash for change event in percentage.", Default = 1)]
        public int DownloadPercentageThrash { get; set; }
        [Option('p', "period", Required = false, HelpText = "Monitor refresh period in second.", Default = 1)]
        public int MonitorPeriodInSecond { get; set; }
    }
}
