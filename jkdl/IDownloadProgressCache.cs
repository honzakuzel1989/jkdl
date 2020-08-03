namespace jkdl
{
    public interface IDownloadProgressCache
    {
        DownloadProcessInfo this[string filename] { get; set; }
        bool TryGetValue(string filename, out DownloadProcessInfo info);
    }
}