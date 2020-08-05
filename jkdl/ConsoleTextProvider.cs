using System;
using System.IO;

namespace jkdl
{
    class ConsoleTextProvider : ITextProvider
    {
        public ConsoleTextProvider()
        {
            Writer = Console.Out;
            Reader = Console.In;
        }

        public TextWriter Writer { get; }
        public TextReader Reader { get; }
    }
}
