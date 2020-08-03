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
        private readonly ILinksProvider _linksProvider;

        public FileDownloader(ILogger<FileDownloader> logger, 
            ILinksProvider linksProvider, 
            IFileNameProvider fileNameProvider,
            IDownloadProgressProvider downloadProgressProvider,
            IWebClientFactory webClientFactory)
        {
            _logger = logger;
            _fileNameProvider = fileNameProvider;
            _downloadProgressProvider = downloadProgressProvider;
            _webClientFactory = webClientFactory;
            _linksProvider = linksProvider;
        }

        public async Task DownloadAsync(string link)
        {
            try
            {
                _logger.LogInformation($"Downloading data from link: {link}");

                var filename = _fileNameProvider.GetFileName(link);

                //if (!File.Exists(filename))
                {
                    using var client = _webClientFactory.CreateWebClient(link, filename);
                    client.OnDownloadProgressInfoChanged += _downloadProgressProvider.DownloadProgressChanged;
                    await client.DownloadFileTaskAsync(link, filename);

                    _logger.LogInformation($"File {filename} successfully downloaded");
                }
                //else
                //{
                //    _logger.LogInformation($"File {filename} already exists!");
                //}
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
