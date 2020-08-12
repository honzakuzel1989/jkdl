using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace jkdl
{
    internal class DownloadProgressCache : IDownloadProgressCache
    {
        private readonly ConcurrentDictionary<string, DownloadProcessInfo> _cache = new ConcurrentDictionary<string, DownloadProcessInfo>();
        private readonly ILogger<DownloadProgressCache> _logger;

        public DownloadProcessInfo this[string key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        public bool IsEmpty => _cache.IsEmpty;

        public IEnumerable<DownloadProcessInfo> Values => _cache.Values;

        public DownloadProgressCache(ILogger<DownloadProgressCache> logger)
        {
            _logger = logger;
        }

        public bool TryGetValue(string key, out DownloadProcessInfo info)
        {
            return _cache.TryGetValue(key, out info);
        }
    }
}
