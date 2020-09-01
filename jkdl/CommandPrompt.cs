using Microsoft.Extensions.Logging;
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
        private readonly IDownloadProgressMonitor _downloadProgressMonitor;
        private readonly ITextProvider _textProvider;
        private readonly IConfigurationService _configurationService;

        private TextReader Reader => _textProvider.Reader;
        private TextWriter Writer => _textProvider.Writer;

        public CommandPrompt(ILogger<CommandPrompt> logger,
            ILinksCache linksCache,
            ILinksProvider linksProvider,
            IFileDownloader fileDownloader,
            IDownloadProgressProvider downloadProgressProvider,
            IDownloadProgressMonitor downloadProgressMonitor,
            ITextProvider textProvider,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _linksCache = linksCache;
            _linksProvider = linksProvider;
            _fileDownloader = fileDownloader;
            _downloadProgressProvider = downloadProgressProvider;
            _downloadProgressMonitor = downloadProgressMonitor;
            _textProvider = textProvider;
            _configurationService = configurationService;
        }

        public async Task Run(CancellationTokenSource cts)
        {
            var line = string.Empty;
            while (!cts.IsCancellationRequested)
            {
                line = Reader.ReadLine();
                switch (line)
                {
                    case "exit":
                        cts.Cancel();
                        break;
                    case "file":
                        var filename = Reader.ReadLine();
                        var flinks = await _linksProvider.GetLinks(new FileInfo(filename));
                        foreach (var flink in flinks)
                            _linksCache.Add(flink, cts.Token);
                        break;
                    case "progress":
                        await _downloadProgressProvider.ReportProgress();
                        break;
                    case "cancel":
                        var key = Reader.ReadLine();
                        await _fileDownloader.CancelDownload(key);
                        break;
                    case "stats":
                        await _downloadProgressProvider.ReportStatistics();
                        break;
                    case "history":
                        await _downloadProgressProvider.ReportHistory();
                        break;
                    case "monitor":
                        var mcommand = Reader.ReadLine();
                        switch (mcommand)
                        {
                            case "start":
                                _downloadProgressMonitor.StartMonitor();
                                break;
                            case "stop":
                                _downloadProgressMonitor.StopMonitor();
                                break;
                            default:
                                Writer.WriteLine("Unknown command. Type \"help\" for available commands.");
                                break;
                        }
                        break;
                    case "help":
                        PrintHelp();
                        break;
                    case "link":
                        var link = Reader.ReadLine();
                        _linksCache.Add(link, cts.Token);
                        break;
                    default:
                        Writer.WriteLine("Unknown command. Type \"help\" for available commands.");
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
            Writer.WriteLine("Available commands:");
            Writer.WriteLine($"  link\t\tinsert link into the cache to download");
            Writer.WriteLine($"  file\t\tinsert filename with links into the cache to download");
            Writer.WriteLine($"  progress\tprint download progress");
            Writer.WriteLine($"  stats\t\tprint download statistics");
            Writer.WriteLine($"  history\tprint download history");
            Writer.WriteLine($"  monitor\t[start] or [stop] monitor download history");
            Writer.WriteLine($"  cancel\tcancel download by download identification");
            Writer.WriteLine($"  exit\t\texit the application");
            Writer.WriteLine($"  help\t\tshow this help message");
        }
    }
}
