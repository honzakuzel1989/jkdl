using System.IO;
using System.Threading.Tasks;

namespace jkdl
{
    internal interface ICommandPrompt
    {
        Task RunAsync(TextReader reader, TextWriter writer);
    }
}