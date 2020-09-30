using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace jkdl
{
    internal class WebClientFactory : IWebClientFactory
    {
        private readonly ConcurrentBag<WebClientData> _webClientsData = new ConcurrentBag<WebClientData>();

        private readonly ILogger<WebClientFactory> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly IConfigurationOptions _configurationService;

        public WebClientFactory(ILogger<WebClientFactory> logger, 
            IDownloadProgressCache downloadProgressCache,
            IConfigurationOptions configurationService)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
            _configurationService = configurationService;
        }

        public IWebClient CreateWebClient(DownloadProcessInfo info)
        {
            // Get first not running client
            if (_webClientsData.FirstOrDefault(wc => !wc.Info.Running) is WebClientData data)
                return data.WebClient;

            // New client
            var newclient = new WebClient(_downloadProgressCache, _configurationService);
            _webClientsData.Add(new WebClientData { WebClient = newclient, Info = info });

            // Max is _configurationService.MaxNumberOfDownload
            if (_webClientsData.Count > _configurationService.MaxNumberOfDownload)
                throw new InvalidOperationException($"Create web client: unexpected number of web clients " +
                    $"(current: {_webClientsData.Count}, max: '{_configurationService.MaxNumberOfDownload}').");

            return newclient;
        }
    }
}