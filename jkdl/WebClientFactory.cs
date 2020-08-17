using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq;

namespace jkdl
{
    internal class WebClientFactory : IWebClientFactory
    {
        private readonly ConcurrentBag<WebClientData> _webClientsData = new ConcurrentBag<WebClientData>();

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
            if (_webClientsData.FirstOrDefault(wc => wc.Info.Running) is IWebClient client)
                return client;

            // Max is _configurationService.MaxNumberOfDownload
            var newclient = new WebClient(info.Key, _downloadProgressCache, _configurationService);
            _webClientsData.Add(new WebClientData { WebClient = newclient, Info = info });

            return newclient;
        }
    }
}