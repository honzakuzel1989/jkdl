using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    class CommandPrompt : ICommandPrompt
    {
        private readonly ILogger<CommandPrompt> _logger;
        private readonly ILinksCache _linksCache;
        private readonly ILinksProvider _linksProvider;
        private readonly IFileDownloader _fileDownloader;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public CommandPrompt(ILogger<CommandPrompt> logger, 
            ILinksCache linksCache, 
            ILinksProvider linksProvider,
            IFileDownloader fileDownloader)
        {
            _logger = logger;
            _linksCache = linksCache;
            _linksProvider = linksProvider;
            _fileDownloader = fileDownloader;
        }

        public async Task RunAsync(TextReader reader, TextWriter writer)
        {
            _ = Task.Run(() => _fileDownloader.Run(cts.Token));

            var cmd = string.Empty;
            while (!cts.IsCancellationRequested)
            {
                writer.Write("> ");
                cmd = reader.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        cts.Cancel();
                        break;
                    case "link":
                        writer.Write("> ");
                        var link = reader.ReadLine();
                        _linksCache.AddLink(link);
                        break;
                    case "file":
                        writer.Write("> ");
                        var filename = reader.ReadLine();
                        foreach (var flink in await _linksProvider.GetLinks(new FileInfo(filename)))
                            _linksCache.AddLink(flink);
                        break;
                }
            }
        }
    }
}
