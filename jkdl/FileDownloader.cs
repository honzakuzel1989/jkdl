using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jkdl
{
    internal class FileDownloader : IFileDownloader
    {
        private readonly ILogger<FileDownloader> _logger;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IDownloadProgressProvider _downloadProgressProvider;
        private readonly IWebClientFactory _webClientFactory;
        private readonly IConfigurationService _configuration;
        private readonly ILinksProvider _linksProvider;

        public FileDownloader(ILogger<FileDownloader> logger, 
            ILinksProvider linksProvider, 
            IFileNameProvider fileNameProvider,
            IDownloadProgressProvider downloadProgressProvider,
            IWebClientFactory webClientFactory,
            IConfigurationService configuration)
        {
            _logger = logger;
            _fileNameProvider = fileNameProvider;
            _downloadProgressProvider = downloadProgressProvider;
            _webClientFactory = webClientFactory;
            _configuration = configuration;
            _linksProvider = linksProvider;
        }

        public async Task DownloadAsync(string link)
        {
            try
            {
                _logger.LogInformation($"Downloading data from link: {link}");

                var filename = _fileNameProvider.GetFileName(link);

                if (!File.Exists(filename) || (File.Exists(filename) && _configuration.Overwrite))
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
        }

        public async Task DownloadAsync(FileInfo fileInfo)
        {
            var links = await _linksProvider.GetLinks(fileInfo);
            var tlinks = links.Select(async link => await DownloadAsync(link));

            await Task.WhenAll(tlinks);
        }
    }
}
