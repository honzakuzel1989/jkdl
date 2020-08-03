using Microsoft.Extensions.Logging;

namespace jkdl
{
    internal class FileNameProvider : IFileNameProvider
    {
        private readonly ILogger<FileNameProvider> _logger;

        public FileNameProvider(ILogger<FileNameProvider> logger)
        {
            _logger = logger;
        }

        public string GetFileName(string link)
        {
            _logger.LogInformation($"\tGetting filename from link: {link}");

            var filename = default(string);

            var tlink = link.Trim();
            var si = tlink.LastIndexOf('/');
            if (si < 0) filename = tlink;
            else filename = tlink.Substring(si + 1);

            _logger.LogInformation($"\t\t{filename}");

            return filename;
        }
    }
}
