namespace jkdl
{
    public interface IConfigurationService
    {
        public int MaxNumberOfDownload { get; }
        public bool OverwriteResult { get; }
        public string DownloadLocation { get; }
        public int DownloadProgressThrash { get; }
    }
}
