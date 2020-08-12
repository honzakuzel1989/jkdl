namespace jkdl
{
    public interface IConfigurationService
    {
        int MonitorPeriodInSecond { get; }
        bool Interactive { get; }
        int MaxNumberOfDownload { get; }
        bool OverwriteResult { get; }
        string DownloadLocation { get; }
        int DownloadProgressThrash { get; }
    }
}
