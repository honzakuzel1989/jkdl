using System.IO;

namespace jkdl
{
    public interface ITextReaderProvider
    {
        TextReader Reader { get; }
    }
}