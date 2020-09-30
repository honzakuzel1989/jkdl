namespace jkdl
{
    public interface IConfigurationOptions
    {
        int MonitorPeriodInSecond { get; }
        bool Background { get; }
        int MaxNumberOfDownload { get; }
        bool OverwriteResults { get; }
        string DownloadLocation { get; }
        int DownloadPercentageThrash { get; }
        string User { get; }
        string Password { get; }
    }
}
