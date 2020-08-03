using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    public interface ILinksProvider
    {
        Task<string[]> GetLinks(FileInfo fileInfo);
    }
}