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
        private readonly IDownloadProgressProvider _downloadProgressProvider;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public CommandPrompt(ILogger<CommandPrompt> logger,
            ILinksCache linksCache,
            ILinksProvider linksProvider,
            IFileDownloader fileDownloader,
            IDownloadProgressProvider downloadProgressProvider)
        {
            _logger = logger;
            _linksCache = linksCache;
            _linksProvider = linksProvider;
            _fileDownloader = fileDownloader;
            _downloadProgressProvider = downloadProgressProvider;
        }

        public async Task RunAsync(TextReader reader, TextWriter writer)
        {
            _ = Task.Run(() => _fileDownloader.Run(cts.Token));

            var cmd = string.Empty;
            while (!cts.IsCancellationRequested)
            {
                cmd = reader.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        cts.Cancel();
                        break;
                    case "link":
                        writer.Write("Insert link to download...");
                        var link = reader.ReadLine();
                        _linksCache.AddLink(link, cts.Token);
                        break;
                    case "file":
                        writer.Write("Insert filename with links to download...");
                        var filename = reader.ReadLine();
                        var flinks = await _linksProvider.GetLinks(new FileInfo(filename));
                        foreach (var flink in flinks)
                            _linksCache.AddLink(flink, cts.Token);
                        break;
                    case "stats":
                        await _downloadProgressProvider.ReportStatistics(writer);
                        break;
                    case "history":
                        await _downloadProgressProvider.ReportHistory(writer);
                        break;
                    case "":
                        writer.WriteLine($"Print {Guid.NewGuid():N} for available commands...");
                        break;
                    default:
                        writer.WriteLine($"Unknown command '{cmd}'...");
                        PrintAvailableCommands(writer);
                        break;
                }
            }
        }

        private void PrintAvailableCommands(TextWriter writer)
        {
            writer.WriteLine("AvailableCommands:");
            writer.WriteLine($"\tlink -\tinsert link into the cache to download");
            writer.WriteLine($"\tfile -\tinsert filename with links into the cache to download");
            writer.WriteLine($"\tstats -\tprint download statistics");
            writer.WriteLine($"\thistory -\tprint download history");
            writer.WriteLine($"\texit -\texit the application");
        }
    }
}
