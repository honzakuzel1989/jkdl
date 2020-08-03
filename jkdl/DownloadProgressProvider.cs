using Microsoft.Extensions.Logging;

namespace jkdl
{
    public class DownloadProgressProvider : IDownloadProgressProvider
    {
        const int MBMULT = 1_000_000;
        const string MB = "MB";

        private readonly ILogger<DownloadProgressProvider> _logger;

        public DownloadProgressProvider(ILogger<DownloadProgressProvider> logger)
        {
            _logger = logger;
        }

        public void DownloadProgressChanged(object sender, DownloadProcessInfoEventArgs e)
        {
            _logger.LogInformation($"\t{e.Info.Filename}\n\t\t{e.Info.ProgressPercentage} [%]\t{e.Info.BytesReceived / MBMULT}/{e.Info.TotalBytesToReceive / MBMULT} [{MB}]");
        }
    }
}
