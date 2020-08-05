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

        private readonly string _link;
        private readonly string _filename;
        private readonly IDownloadProgressCache _progressCache;

        public WebClient(string link, string filename, IDownloadProgressCache progressCache)
        {
            _link = link;
            _filename = filename;
            _progressCache = progressCache;

            DownloadProgressChanged += WebClientWrapper_DownloadProgressChanged;
            DownloadFileCompleted += WebClient_DownloadFileCompleted;
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_filename, out var info))
                {
                    var eacompleted = new DownloadProcessCompletedEventArgs(e, info);
                    _progressCache[_filename] = eacompleted.Info;
                    OnDownloadProgressCompleted?.Invoke(this, eacompleted);
                }
            }
        }

        private void WebClientWrapper_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_filename, out var info) &&
                    e.ProgressPercentage > info.ProgressPercentage)
                {
                    var eainfo = new DownloadProcessInfoEventArgs(e, _link, _filename);
                    _progressCache[_filename] = eainfo.Info;
                    OnDownloadProgressInfoChanged?.Invoke(this, eainfo);
                }
            }
        }
    }
}
