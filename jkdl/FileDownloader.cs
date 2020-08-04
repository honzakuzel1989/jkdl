using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    internal class FileDownloader : IFileDownloader
    {
        private volatile int NUMBER_OF_DOWLOANDS = 0;

        private readonly ILogger<FileDownloader> _logger;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IDownloadProgressProvider _downloadProgressProvider;
        private readonly IWebClientFactory _webClientFactory;
        private readonly IConfigurationService _configuration;
        private readonly ILinksCache _linksCache;
        private readonly ILinksProvider _linksProvider;

        public FileDownloader(ILogger<FileDownloader> logger,
            ILinksProvider linksProvider,
            IFileNameProvider fileNameProvider,
            IDownloadProgressProvider downloadProgressProvider,
            IWebClientFactory webClientFactory,
            IConfigurationService configuration,
            ILinksCache linksCache)
        {
            _logger = logger;
            _fileNameProvider = fileNameProvider;
            _downloadProgressProvider = downloadProgressProvider;
            _webClientFactory = webClientFactory;
            _configuration = configuration;
            _linksCache = linksCache;
            _linksProvider = linksProvider;
        }

        public async Task DownloadAsync(string link)
        {
            NUMBER_OF_DOWLOANDS++;

            try
            {
                _logger.LogInformation($"Downloading data from link: {link}");
                var filename = _fileNameProvider.GetFileName(link);

                if (!File.Exists(filename) || (File.Exists(filename) && _configuration.OverwriteResult))
                {
                    using var client = _webClientFactory.CreateWebClient(link, filename);
                    client.OnDownloadProgressInfoChanged += _downloadProgressProvider.DownloadProgressChanged;
                    await client.DownloadFileTaskAsync(link, filename);

                    _logger.LogInformation($"File {filename} successfully downloaded");
                }
                else
                {
                    _logger.LogWarning($"File {filename} already exists!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                NUMBER_OF_DOWLOANDS--;
            }
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Downloader started...");

                // Get link from blocking collection
                foreach (var link in _linksCache.GetLinks(cancellationToken))
                {
                    // Download link
                    await DownloadAsync(link);

                    // Wait for empty download slot
                    while (NUMBER_OF_DOWLOANDS > _configuration.MaxNumberOfDownload)
                        await Task.Delay(500);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Downloader ended...");
            }
        }
    }
}
