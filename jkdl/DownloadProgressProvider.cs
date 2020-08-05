using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    public class DownloadProgressProvider : IDownloadProgressProvider
    {
        const int MBMULT = 1_000_000;
        const string MB = "MB";

        private readonly ILogger<DownloadProgressProvider> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly ITextProvider _textProvider;

        private TextWriter Writer => _textProvider.Writer;

        public DownloadProgressProvider(ILogger<DownloadProgressProvider> logger,
            IDownloadProgressCache downloadProgressCache,
            ITextProvider textProvider)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
            _textProvider = textProvider;
        }

        public async Task ReportProgress()
        {
            bool empty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (!info.Completed)
                {
                    await Writer.WriteLineAsync($"\t{info.Filename}\n\t\t{info.ProgressPercentage} [%]\t{info.BytesReceived / MBMULT}/{info.TotalBytesToReceive / MBMULT} [{MB}]");
                    empty = false;
                }
            }

            if (empty)
            {
                Writer.WriteLine("\tNo current download...");
            }
        }

        public async Task ReportHistory()
        {
            bool empty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (info.Completed)
                {
                    string result = "Ok";
                    if (info.Cancelled) result = "Cancelled";
                    if (info.Failed) result = "Failed";

                    await Writer.WriteLineAsync($"\t{info.Filename}\n\t\t{info.ProgressPercentage} [%]\t{result}");
                    empty = false;
                }
            }

            if (empty)
            {
                Writer.WriteLine("\tNo history yet...");
            }
        }

        public async Task ReportStatistics()
        {
            var active = 0;
            var downloaded = 0;
            var downloadedSize = (long)0;
            var cancelled = 0;
            var waiting = 0;
            var failed = 0;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (info.Completed)
                {
                    if (info.Cancelled)
                    {
                        cancelled++;
                    }
                    else if (info.Failed)
                    {
                        failed++;
                    }
                    else
                    {
                        downloaded++;
                        downloadedSize += info.BytesReceived / MBMULT;
                    }
                }
                else
                {
                    if (info.Running)
                    {
                        active++;
                    }
                    else
                    {
                        waiting++;
                    }
                }
            }

            await Writer.WriteLineAsync($"\t" +
                $"Active: {active}\n\t" +
                $"Downloaded: {downloaded}\n\t" +
                $"Downloaded size: {downloadedSize} [{MB}]\n\t" +
                $"Waiting: {waiting}\n\t" +
                $"Failed: {failed}\n\t" +
                $"Cancelled: {cancelled}");
        }
    }
}
