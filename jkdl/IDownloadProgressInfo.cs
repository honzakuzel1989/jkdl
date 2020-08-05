using System.Threading.Tasks;

namespace jkdl
{
    public interface IDownloadProgressProvider
    {
        Task ReportStatistics();
        Task ReportHistory();
    }
}