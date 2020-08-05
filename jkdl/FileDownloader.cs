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
        private readonly IOutputFileNameProvider _ofileNameProvider;
        private readonly IDownloadProgressProvider _downloadProgressProvider;
        private readonly IWebClientFactory _webClientFactory;
        private readonly IConfigurationService _configuration;
        private readonly ILinksCache _linksCache;
        private readonly ITextProvider _textProvider;
        private readonly ILinksProvider _linksProvider;

        public FileDownloader(ILogger<FileDownloader> logger,
            ILinksProvider linksProvider,
            IOutputFileNameProvider ofileNameProvider,
            IDownloadProgressProvider downloadProgressProvider,
            IWebClientFactory webClientFactory,
            IConfigurationService configuration,
            ILinksCache linksCache,
            ITextProvider textProvider)
        {
            _logger = logger;
            _ofileNameProvider = ofileNameProvider;
            _downloadProgressProvider = downloadProgressProvider;
            _webClientFactory = webClientFactory;
            _configuration = configuration;
            _linksCache = linksCache;
            _textProvider = textProvider;
            _linksProvider = linksProvider;
        }

        private async Task DownloadAsync(DownloadProcessInfo info)
        {
            try
            {
                _logger.LogInformation($"Downloading data from link: {info.Link} to {info.Filename}...");

                if (!File.Exists(info.Filename) || (File.Exists(info.Filename) && _configuration.OverwriteResult))
                {
                    using var client = _webClientFactory.CreateWebClient(info);
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
                _logger.LogError(ex.ToString());
                _textProvider.Writer.WriteLine($"Error: {ex.Message}");

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
                    _ = Task.Run(async () => await DownloadAsync(link))
                        .ContinueWith(_ => numberOfDownloads--);

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
    }
}
