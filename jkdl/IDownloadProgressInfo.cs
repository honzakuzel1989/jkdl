namespace jkdl
{
    public interface IDownloadProgressProvider
    {
        void DownloadProgressChanged(object sender, DownloadProcessInfoEventArgs e);
    }
}