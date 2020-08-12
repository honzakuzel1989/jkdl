using System;
using System.Threading.Tasks;

namespace jkdl
{
    public interface IWebClient : IDisposable
    {
        event EventHandler<DownloadProcessInfoEventArgs> OnDownloadProgressInfoChanged;
        event EventHandler<DownloadProcessCompletedEventArgs> OnDownloadProgressCompleted;

        Task DownloadFileTaskAsync(string link, string filename);
        void CancelAsync();
    }
}