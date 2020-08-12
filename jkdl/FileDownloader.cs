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
        private readonly ITextProvider _textProvider;
        private readonly IDownloadClientsCache _downloadClientsCache;
        private readonly INotificationService _notificationService;

        public FileDownloader(ILogger<FileDownloader> logger,
            IWebClientFactory webClientFactory,
            IConfigurationService configuration,
            ILinksCache linksCache,
            ITextProvider textProvider,
            IDownloadClientsCache downloadClientsCache,
            INotificationService notificationService)
        {
            _logger = logger;
            _webClientFactory = webClientFactory;
            _configuration = configuration;
            _linksCache = linksCache;
            _textProvider = textProvider;
            _downloadClientsCache = downloadClientsCache;
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
                    _downloadClientsCache[info.Key] = client;
                    
                    await client.DownloadFileTaskAsync(info.Link, info.Filename);

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
            finally
            {
                _downloadClientsCache[info.Key].CancelAsync();
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
            if (_downloadClientsCache.TryGetValue(key, out var client))
            {
                // TODO: cancel download
                //await Task.Run(() => client.CancelAsync());
            }
            else
            {
                await _notificationService.ProccessError(_logger, $"Client for download with key {key} not found.");
            }

        }
    }
}
