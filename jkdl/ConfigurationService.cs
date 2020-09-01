namespace jkdl
{
    internal class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(string[] args)
        {
        }

        public bool Interactive => true;
        public bool OverwriteResults => true;
        public int MaxNumberOfDownload => 3;
        public string DownloadLocation => ".";
        public int DownloadPercentageThrash => 1;
        public int MonitorPeriodInSecond => 1;
    }
}
