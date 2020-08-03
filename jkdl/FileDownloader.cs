using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace jkdl
{
    internal class FileDownloader : IFileDownloader
    {
        private readonly ILogger<FileDownloader> _logger;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly ILinksProvider _linksProvider;

        private readonly ConcurrentDictionary<string, DownloadInfo> _progress = new ConcurrentDictionary<string, DownloadInfo>();

        public FileDownloader(ILogger<FileDownloader> logger, 
            ILinksProvider linksProvider, 
            IFileNameProvider fileNameProvider)
        {
            _logger = logger;
            _fileNameProvider = fileNameProvider;
            _linksProvider = linksProvider;
        }

        public async Task DownloadAsync(string link)
        {
            try
            {
                _logger.LogInformation($"Downloading data from link: {link}");

                var filename = _fileNameProvider.GetFileName(link);
                _progress[filename] = new DownloadInfo(link);

                //if (!File.Exists(filename))
                {
                    using var client = new WebClient();
                    client.DownloadProgressChanged += (_, e) =>
                    {
                        if (e.ProgressPercentage > _progress[filename].ProgressPercentage)
                        {
                            const int mult = 1_000_000;
                            const string suff = "MB";

                            _progress[filename].BytesReceived = e.BytesReceived;
                            _progress[filename].TotalBytesToReceive = e.TotalBytesToReceive;
                            _progress[filename].ProgressPercentage = e.ProgressPercentage;

                            _progress[filename].ProgressPercentage = e.ProgressPercentage;
                            _logger.LogInformation($"\t{filename}\n\t\t{e.ProgressPercentage} [%]\t{e.BytesReceived / mult}/{e.TotalBytesToReceive / mult} [{suff}]");
                        }
                    };

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
