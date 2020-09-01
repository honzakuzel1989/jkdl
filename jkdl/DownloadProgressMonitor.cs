using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace jkdl
{
    class DownloadProgressMonitor : IDownloadProgressMonitor
    {
        private readonly ILogger<DownloadProgressMonitor> _logger;
        private readonly IDownloadProgressProvider _downloadProgressProvider;
        private readonly IDownloadProgressCache _downloadProgressCache;
        private readonly ITextProvider _textProvider;
        private bool _disposedValue;
        private readonly Timer _progressTimer;

        public DownloadProgressMonitor(ILogger<DownloadProgressMonitor> logger,
            IDownloadProgressProvider downloadProgressProvider,
            IConfigurationOptions configurationService,
            IDownloadProgressCache downloadProgressCache,
            ITextProvider textProvider)
        {
            _logger = logger;
            _downloadProgressProvider = downloadProgressProvider;
            _downloadProgressCache = downloadProgressCache;
            _textProvider = textProvider;

            _progressTimer = new Timer(TimeSpan.FromSeconds(configurationService.MonitorPeriodInSecond).TotalMilliseconds);
            _progressTimer.Elapsed += ProgressTimer_Elapsed;
        }

        private async void ProgressTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_downloadProgressCache.Values.Any(i => i.Running))
            {
                await _downloadProgressProvider.ReportProgress();
                await _textProvider.Writer.WriteLineAsync();
            }
            else
            {
                await _downloadProgressProvider.ReportProgress();
            }
        }

        public void StartMonitor()
        {
            _progressTimer.Start();
        }

        public void StopMonitor()
        {
            _progressTimer.Stop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _progressTimer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
