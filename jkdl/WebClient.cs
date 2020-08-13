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
        }

        private void DownloadCompleted(Exception exception, bool cancelled)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_processKey, out var info))
                {
                    var eacompleted = new DownloadProcessCompletedEventArgs(exception, cancelled, info);
                    _progressCache[_processKey] = eacompleted.Info;
                    OnDownloadProgressCompleted?.Invoke(this, eacompleted);
                }
            }
        }

        private void DownloadProgress(long received, long toReceive, int percentage)
        {
            lock (_progressLock)
            {
                if (_progressCache.TryGetValue(_processKey, out var info)
                    && percentage - info.ProgressPercentage >= _configurationService.DownloadProgressThrash)
                {
                    var eainfo = new DownloadProcessInfoEventArgs(received, toReceive, percentage, info);
                    _progressCache[_processKey] = eainfo.Info;
                    OnDownloadProgressInfoChanged?.Invoke(this, eainfo);
                }
            }
        }

        public async Task DownloadFile(string link, string filename, CancellationToken token)
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
                    DownloadProgress(receivedBytes, totalBytesToReceive,
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
                DownloadCompleted(exception, token.IsCancellationRequested);
            }
        }
    }
}
