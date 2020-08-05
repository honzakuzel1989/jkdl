using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace jkdl
{
    internal class LinksCache : ILinksCache
    {
        private readonly BlockingCollection<DownloadProcessInfo> _linksCache = new BlockingCollection<DownloadProcessInfo>();
        private readonly ILogger<LinksCache> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly IOutputFileNameProvider _outputFileNameProvider;

        public int Count => _linksCache.Count;

        public LinksCache(ILogger<LinksCache> logger,
            IDownloadProgressCache downloadProgressCache,
            IOutputFileNameProvider outputFileNameProvider)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
            _outputFileNameProvider = outputFileNameProvider;
        }

        public void Add(string link, CancellationToken cancellationToken)
        {
            var filename = _outputFileNameProvider.GetAbsoluteFileName(link);
            var info = new DownloadProcessInfo(link, filename);

            _downloadProgressCache[filename] = info;
            _linksCache.Add(info, cancellationToken);

            _logger.LogInformation($"Inserted new link {link}.");
        }

        public IEnumerable<DownloadProcessInfo> Get(CancellationToken cancellationToken)
        {
            return _linksCache.GetConsumingEnumerable(cancellationToken);
        }
    }
}