using Microsoft.Extensions.Logging;

namespace jkdl
{
    internal class WebClientFactory : IWebClientFactory
    {
        private readonly ILogger<WebClientFactory> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly IConfigurationService _configurationService;

        public WebClientFactory(ILogger<WebClientFactory> logger, 
            IDownloadProgressCache downloadProgressCache,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
            _configurationService = configurationService;
        }

        public IWebClient CreateWebClient(DownloadProcessInfo info)
        {
            return new WebClient(info.Key, _downloadProgressCache, _configurationService);
        }
    }
}