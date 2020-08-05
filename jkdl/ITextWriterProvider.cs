using System.IO;

namespace jkdl
{
    public interface ITextWriterProvider
    {
        TextWriter Writer { get; }
    }
}