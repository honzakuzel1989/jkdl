using System.Collections.Generic;
using System.Threading;

namespace jkdl
{
    public interface ILinksCache
    {
        int Count { get; }
        void Add(string link, CancellationToken cancellationToken);
        IEnumerable<DownloadProcessInfo> Get(CancellationToken cancellationToken);
    }
}