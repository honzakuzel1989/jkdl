using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    public interface IFileDownloader
    {
        Task Run(CancellationToken cancellationToken);
        Task CancelDownload(string key);
    }
}