using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    internal interface ICommandPrompt
    {
        Task Run(CancellationTokenSource cts);
    }
}