using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    internal class WebClient : System.Net.WebClient, IWebClient
    {
        public event EventHandler<DownloadProcessInfoEventArgs> OnDownloadProgressInfoChanged;
        public event EventHandler<DownloadProcessCompletedEventArgs> OnDownloadProgressCompleted;

        private readonly object _progressLock = new object();

        private readonly IDownloadProgressCache _progressCache;
        private readonly IConfigurationOptions _configurationService;

        public WebClient(IDownloadProgressCache progressCache,
            IConfigurationOptions configurationService)
        {
            _progressCache = progressCache;
            _configurationService = configurationService;

            if (!string.IsNullOrEmpty(_configurationService.User) && !string.IsNullOrEmpty(_configurationService.Password))
                Credentials = new System.Net.NetworkCredential(_configurationService.User, _configurationService.Password);
        }

        private void DownloadCompleted(string key, Exception exception, bool cancelled)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(key, out var info))
                {
                    var eacompleted = new DownloadProcessCompletedEventArgs(exception, cancelled, info);
                    _progressCache[key] = eacompleted.Info;
                    OnDownloadProgressCompleted?.Invoke(this, eacompleted);
                }
            }
        }

        private void DownloadProgress(string key, long received, long toReceive, int percentage)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(key, out var info)
                    && percentage - info.ProgressPercentage >= _configurationService.DownloadPercentageThrash)
                {
                    var eainfo = new DownloadProcessInfoEventArgs(received, toReceive, percentage, info);
                    _progressCache[key] = eainfo.Info;
                    OnDownloadProgressInfoChanged?.Invoke(this, eainfo);
                }
            }
        }

        public async Task DownloadFile(string key, string link, string filename, CancellationToken token)
        {
            Exception exception = null;

            try
            {
                // Buffer
                int buffersize = 2 << 14;
                byte[] buffer = new byte[buffersize];

                // Streams
                using var readstream = await OpenReadTaskAsync(link);
                using var writestream = File.OpenWrite(filename);

                // Total received size and to receive
                long receivedBytes = 0;
                var totalBytesToReceive = long.Parse(ResponseHeaders["Content-Length"]);

                // Read from link
                int count = 0;
                while ((count = await readstream.ReadAsync(buffer, token)) > 0)
                {
                    // Write to file
                    await writestream.WriteAsync(buffer, 0, count, token);

                    // Progress
                    receivedBytes += count;
                    DownloadProgress(key, receivedBytes, totalBytesToReceive,
                        (int)(receivedBytes / (totalBytesToReceive / 100)));
                }
            }
            catch (OperationCanceledException)
            {
                // Empty
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                // Done
                DownloadCompleted(key, exception, token.IsCancellationRequested);
            }
        }
    }
}
