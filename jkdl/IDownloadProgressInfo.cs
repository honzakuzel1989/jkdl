using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    public interface IDownloadProgressProvider
    {
        Task ReportStatistics(TextWriter writer);
        Task ReportHistory(TextWriter writer);
    }
}