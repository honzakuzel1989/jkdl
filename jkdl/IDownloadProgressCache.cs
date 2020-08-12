using System.Collections.Generic;

namespace jkdl
{
    public interface IDownloadProgressCache
    {
        bool IsEmpty { get; }
        IEnumerable<DownloadProcessInfo> Values { get; }
        DownloadProcessInfo this[string key] { get; set; }
        bool TryGetValue(string key, out DownloadProcessInfo info);
    }
}