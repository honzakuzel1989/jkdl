using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace jkdl
{
    internal class LinksCache : ILinksCache
    {
        private readonly BlockingCollection<string> _cache = new BlockingCollection<string>();
        private readonly ILogger<LinksCache> _logger;

        public LinksCache(ILogger<LinksCache> logger)
        {
            _logger = logger;
        }

        public void AddLink(string link, CancellationToken cancellationToken)
        {
            _cache.Add(link, cancellationToken);
            _logger.LogInformation($"Inserted new link {link}.");
        }

        public IEnumerable<string> GetLinks(CancellationToken cancellationToken)
        {
            return _cache.GetConsumingEnumerable(cancellationToken);
        }
    }
}