using System;
using System.ComponentModel;
using System.Net;

namespace jkdl
{
    internal class WebClient : System.Net.WebClient, IWebClient
    {
        public event EventHandler<DownloadProcessInfoEventArgs> OnDownloadProgressInfoChanged;
        public event EventHandler<DownloadProcessCompletedEventArgs> OnDownloadProgressCompleted;

        private readonly object _progressLock = new object();

        private readonly string _processKey;

        private readonly IDownloadProgressCache _progressCache;
        private readonly IConfigurationService _configurationService;

        public WebClient(string processKey, 
            IDownloadProgressCache progressCache,
            IConfigurationService configurationService)
        {
            _processKey = processKey;
            
            _progressCache = progressCache;
            _configurationService = configurationService;

            DownloadProgressChanged += WebClientWrapper_DownloadProgressChanged;
            DownloadFileCompleted += WebClient_DownloadFileCompleted;
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_processKey, out var info))
                {
                    var eacompleted = new DownloadProcessCompletedEventArgs(e, info);
                    _progressCache[_processKey] = eacompleted.Info;
                    OnDownloadProgressCompleted?.Invoke(this, eacompleted);
                }
            }
        }

        private void WebClientWrapper_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_processKey, out var info) &&
                    e.ProgressPercentage - info.ProgressPercentage >= _configurationService.DownloadProgressThrash)
                {
                    var eainfo = new DownloadProcessInfoEventArgs(e, info);
                    _progressCache[_processKey] = eainfo.Info;
                    OnDownloadProgressInfoChanged?.Invoke(this, eainfo);
                }
            }
        }
    }
}
