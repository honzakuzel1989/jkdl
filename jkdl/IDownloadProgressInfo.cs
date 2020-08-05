using System.Threading.Tasks;

namespace jkdl
{
    public interface IDownloadProgressProvider
    {
        Task ReportProgress();
        Task ReportHistory();
        Task ReportStatistics();
    }
}