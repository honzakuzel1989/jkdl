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

        public DownloadProgressProvider(ILogger<DownloadProgressProvider> logger, IDownloadProgressCache downloadProgressCache)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
        }

        public async Task ReportStatistics(TextWriter writer)
        {
            bool empty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (!info.Completed)
                {
                    await writer.WriteLineAsync($"\t{info.Filename}\n\t\t{info.ProgressPercentage} [%]\t{info.BytesReceived / MBMULT}/{info.TotalBytesToReceive / MBMULT} [{MB}]");
                    empty = false;
                }
            }

            if (empty)
            {
                writer.WriteLine("No statistics yet...");
            }
        }

        public async Task ReportHistory(TextWriter writer)
        {
            bool empty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (info.Completed)
                {
                    await writer.WriteLineAsync($"\t{info.Filename}\n\t\t{info.ProgressPercentage} [%]\t{(info.Cancelled ? "Cancelled" : "Ok")}");
                    empty = false;
                }
            }

            if (empty)
            {
                writer.WriteLine("No history yet...");
            }
        }
    }
}
