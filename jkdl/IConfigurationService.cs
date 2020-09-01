namespace jkdl
{
    public interface IConfigurationService
    {
        int MonitorPeriodInSecond { get; }
        bool Interactive { get; }
        int MaxNumberOfDownload { get; }
        bool OverwriteResults { get; }
        string DownloadLocation { get; }
        int DownloadPercentageThrash { get; }
    }
}
