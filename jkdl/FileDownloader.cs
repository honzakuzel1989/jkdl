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

        public async Task DownloadAsync(string link)
        {
            NUMBER_OF_DOWLOANDS++;

            try
            {
                _logger.LogInformation($"Downloading data from link: {link}");
                var ofilename = _ofileNameProvider.GetAbsoluteFileName(link);

                if (!File.Exists(ofilename) || (File.Exists(ofilename) && _configuration.OverwriteResult))
                {
                    using var client = _webClientFactory.CreateWebClient(link, ofilename);
                    await client.DownloadFileTaskAsync(link, ofilename);

                    _logger.LogInformation($"File {ofilename} successfully downloaded");
                }
                else
                {
                    _logger.LogWarning($"File {ofilename} already exists!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _textProvider.Writer.WriteLine($"Error: {ex.Message}");

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
                    _ = Task.Run(() => DownloadAsync(link));

                    // Wait for empty download slot
                    while (NUMBER_OF_DOWLOANDS > _configuration.MaxNumberOfDownload)
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Downloader ended...");
            }
        }
    }
}
