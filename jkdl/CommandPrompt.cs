﻿using Microsoft.Extensions.Logging;
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
        private readonly ITextProvider _textProvider;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private TextReader Reader => _textProvider.Reader;
        private TextWriter Writer => _textProvider.Writer;

        public CommandPrompt(ILogger<CommandPrompt> logger,
            ILinksCache linksCache,
            ILinksProvider linksProvider,
            IFileDownloader fileDownloader,
            IDownloadProgressProvider downloadProgressProvider,
            ITextProvider textProvider)
        {
            _logger = logger;
            _linksCache = linksCache;
            _linksProvider = linksProvider;
            _fileDownloader = fileDownloader;
            _downloadProgressProvider = downloadProgressProvider;
            _textProvider = textProvider;
        }

        public async Task RunAsync()
        {
            _ = Task.Run(() => _fileDownloader.Run(cts.Token));

            var cmd = string.Empty;
            while (!cts.IsCancellationRequested)
            {
                cmd = Reader.ReadLine();
                switch (cmd)
                {
                    case "exit":
                        cts.Cancel();
                        break;
                    case "link":
                        var link = Reader.ReadLine();
                        _linksCache.AddLink(link, cts.Token);
                        break;
                    case "file":
                        var filename = Reader.ReadLine();
                        var flinks = await _linksProvider.GetLinks(new FileInfo(filename));
                        foreach (var flink in flinks)
                            _linksCache.AddLink(flink, cts.Token);
                        break;
                    case "progress":
                        await _downloadProgressProvider.ReportProgress();
                        break;
                    case "stats":
                        await _downloadProgressProvider.ReportStatistics();
                        break;
                    case "history":
                        await _downloadProgressProvider.ReportHistory();
                        break;
                    case "":
                        Writer.WriteLine($"Print {Guid.NewGuid():N} for available commands...");
                        break;
                    case "help":
                        PrintHelp();
                        break;
                    default:
                        Writer.WriteLine($"Unknown command '{cmd}'...");
                        PrintAvailableCommands();
                        break;
                }
            }
        }

        private void PrintHelp()
        {
            PrintAvailableCommands();
        }

        private void PrintAvailableCommands()
        {
            Writer.WriteLine("AvailableCommands:");
            Writer.WriteLine($"\tlink - insert link into the cache to download");
            Writer.WriteLine($"\tfile - insert filename with links into the cache to download");
            Writer.WriteLine($"\tprogress - print download progress");
            Writer.WriteLine($"\tstats - print download statistics");
            Writer.WriteLine($"\thistory - print download history");
            Writer.WriteLine($"\texit - exit the application");
        }
    }
}
