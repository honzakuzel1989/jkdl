using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace jkdl
{
    internal class DownloadClientsCache : IDownloadClientsCache
    {
        private readonly ConcurrentDictionary<string, IWebClient> _cache = new ConcurrentDictionary<string, IWebClient>();
        private readonly ILogger<DownloadClientsCache> _logger;

        public IWebClient this[string key]
        {
            get => _cache[key];
            set => _cache[key] = value;
        }

        public bool IsEmpty => _cache.IsEmpty;

        public IEnumerable<IWebClient> Values => _cache.Values;

        public DownloadClientsCache(ILogger<DownloadClientsCache> logger)
        {
            _logger = logger;
        }

        public bool TryGetValue(string key, out IWebClient client)
        {
            return _cache.TryGetValue(key, out client);
        }
    }
}
