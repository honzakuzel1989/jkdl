using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace jkdl
{
    internal class DownloadProgressCache : IDownloadProgressCache
    {
        private readonly ConcurrentDictionary<string, DownloadProcessInfo> _cache = new ConcurrentDictionary<string, DownloadProcessInfo>();
        private readonly ILogger<DownloadProgressCache> _logger;

        public DownloadProcessInfo this[string filename]
        {
            get => _cache[filename];
            set => _cache[filename] = value;
        }

        public IEnumerable<DownloadProcessInfo> Values => _cache.Values;

        public DownloadProgressCache(ILogger<DownloadProgressCache> logger)
        {
            _logger = logger;
        }

        public bool TryGetValue(string filename, out DownloadProcessInfo info)
        {
            return _cache.TryGetValue(filename, out info);
        }
    }
}
