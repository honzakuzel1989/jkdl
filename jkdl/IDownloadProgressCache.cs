using System.Collections.Generic;

namespace jkdl
{
    public interface IDownloadProgressCache
    {
        IEnumerable<DownloadProcessInfo> Values { get; }
        DownloadProcessInfo this[string filename] { get; set; }
        bool TryGetValue(string filename, out DownloadProcessInfo info);
    }
}