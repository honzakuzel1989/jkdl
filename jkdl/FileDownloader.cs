using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    internal class FileDownloader : IFileDownloader
    {
        private readonly ILogger<FileDownloader> _logger;
        private readonly IWebClientFactory _webClientFactory;
        private readonly IConfigurationService _configuration;
        private readonly ILinksCache _linksCache;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly INotificationService _notificationService;

        public FileDownloader(ILogger<FileDownloader> logger,
            IWebClientFactory webClientFactory,
            IConfigurationService configuration,
            ILinksCache linksCache,
            IDownloadProgressCache downloadProgressCache,
            INotificationService notificationService)
        {
            _logger = logger;
            _webClientFactory = webClientFactory;
            _configuration = configuration;
            _linksCache = linksCache;
            _downloadProgressCache = downloadProgressCache;
            _notificationService = notificationService;
        }

        private async Task DownloadAsync(DownloadProcessInfo info)
        {
            try
            {
                _logger.LogInformation($"Downloading data from link: {info.Link} to {info.Filename}...");

                if (!File.Exists(info.Filename) || (File.Exists(info.Filename) && _configuration.OverwriteResult))
                {
                    using var client = _webClientFactory.CreateWebClient(info);
                    await client.DownloadFile(info.Link, info.Filename, info.TokenSource.Token);

                    _logger.LogInformation($"File {info.Filename} successfully downloaded");
                }
                else
                {
                    _logger.LogWarning($"File {info.Filename} already exists and overwrite is not allowed!");
                }
            }
            catch (Exception ex)
            {
                await _notificationService.ProccessError(_logger, ex);
            }
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Downloader started...");

                // Get link from blocking collection
                var numberOfDownloads = 0;
                foreach (var link in _linksCache.Get(cancellationToken))
                {
                    // Download link - at least one
                    numberOfDownloads++;
                    _ = Task.Run(async () => await DownloadAsync(link)).ContinueWith(_ => numberOfDownloads--);

                    // Wait for empty download slot
                    while (numberOfDownloads >= _configuration.MaxNumberOfDownload)
                        await Task.Delay(TimeSpan.FromMilliseconds(1000));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Downloader ended...");
            }
        }

        public async Task CancelDownload(string key)
        {
            if (_downloadProgressCache.TryGetValue(key, out var info))
            {
                info.TokenSource.Cancel();
            }
            else
            {
                await _notificationService.ProccessError(_logger, $"Download with key {key} not found.");
            }

        }
    }
}
