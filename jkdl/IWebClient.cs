using System;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    public interface IWebClient : IDisposable
    {
        event EventHandler<DownloadProcessInfoEventArgs> OnDownloadProgressInfoChanged;
        event EventHandler<DownloadProcessCompletedEventArgs> OnDownloadProgressCompleted;

        Task DownloadFile(string link, string filename, CancellationToken token);
    }
}