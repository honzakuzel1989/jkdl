using Microsoft.Extensions.Logging;

namespace jkdl
{
    internal class WebClientFactory : IWebClientFactory
    {
        private readonly ILogger<WebClientFactory> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;

        public WebClientFactory(ILogger<WebClientFactory> logger, IDownloadProgressCache downloadProgressCache)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
        }

        public IWebClient CreateWebClient(string link, string filename)
        {
            return new WebClient(link, filename, _downloadProgressCache);
        }
    }
}