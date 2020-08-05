namespace jkdl
{
    internal class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(string[] args)
        {
        }

        public bool OverwriteResult => true;
        public int MaxNumberOfDownload => 1;
        public string DownloadLocation => ".";
        public int DownloadProgressThrash => 1;
    }
}
