using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    public interface IFileDownloader
    {
        Task DownloadAsync(string link);
        Task DownloadAsync(FileInfo fileInfo);
    }
}