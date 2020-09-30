using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    public class DownloadProgressProvider : IDownloadProgressProvider
    {
        const int MBMULT = 1024 * 1024;
        const string MB = "MB";

        private readonly ILogger<DownloadProgressProvider> _logger;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly IConfigurationOptions _configurationService;
        private readonly ITextProvider _textProvider;

        private TextWriter Writer => _textProvider.Writer;
        private TextReader Reader => _textProvider.Reader;

        public DownloadProgressProvider(ILogger<DownloadProgressProvider> logger,
            IDownloadProgressCache downloadProgressCache,
            IConfigurationOptions configurationService,
            ITextProvider textProvider)
        {
            _logger = logger;
            _downloadProgressCache = downloadProgressCache;
            _configurationService = configurationService;
            _textProvider = textProvider;
        }

        public async Task ReportProgress()
        {
            bool cacheWasEmpty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (!info.Completed)
                {
                    var duration = info.CalculateDuration();
                    var mbreceived = info.BytesReceived / MBMULT;
                    var mbtotal = info.TotalBytesToReceive / MBMULT;
                    var mbspeed = Math.Round(mbreceived / duration.TotalSeconds, 2, MidpointRounding.AwayFromZero);
                    var estimatedtime = mbspeed > 0 ? ((mbtotal - mbreceived) / mbspeed) : double.NaN;

                    await Writer.WriteLineAsync($"" +
                        $"\t== [{info.Key}]" +
                        $"\n\t{info.Filename}{(!info.Running ? " - waiting" : string.Empty)}" +
                        $"\n\t{info.ProgressPercentage} [%] ({duration})" +
                        $"\n\t{mbreceived}/{mbtotal} [{MB}]" +
                        $"\n\t{mbspeed} [{MB}/s]" +
                        $"\n\t{(double.IsNaN(mbspeed) ? double.NaN.ToString() : TimeSpan.FromSeconds((int)estimatedtime).ToString())}");

                    cacheWasEmpty = false;
                }
            }

            if (cacheWasEmpty)
            {
                Writer.WriteLine("\tNo current download...");
            }
        }

        public async Task ReportHistory()
        {
            bool cacheWasEmpty = true;

            foreach (var info in _downloadProgressCache.Values)
            {
                if (info.Completed)
                {
                    string result = "Ok";
                    if (info.Cancelled) result = "Cancelled";
                    if (info.Failed) result = "Failed";

                    await Writer.WriteLineAsync(
                        $"\t[{info.Key}] {info.Filename}" +
                        $"\n\t{info.StartTime} - {info.EndTime} [{info.CalculateDuration()}]" +
                        $"\n\t{info.ProgressPercentage} [%]\t{result}");

                    cacheWasEmpty = false;
                }
            }

            if (cacheWasEmpty)
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
            var downloadedTime = TimeSpan.Zero;

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
                        downloadedTime += info.CalculateDuration();
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
                $"Total size: {downloadedSize} [{MB}]\n\t" +
                $"Total time: {new TimeSpan(downloadedTime.Days, downloadedTime.Hours, downloadedTime.Minutes, downloadedTime.Seconds)}\n\t" +
                $"Average speed: {Math.Round(downloadedSize / downloadedTime.TotalSeconds, 2, MidpointRounding.AwayFromZero)} [{MB}/s]\n\t" +
                $"Waiting: {waiting}\n\t" +
                $"Failed: {failed}\n\t" +
                $"Cancelled: {cancelled}");
        }
    }
}
