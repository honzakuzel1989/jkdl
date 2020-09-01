using Microsoft.Extensions.Logging;
using System.IO;

namespace jkdl
{
    internal class OutputFileNameProvider : IOutputFileNameProvider
    {
        private readonly ILogger<OutputFileNameProvider> _logger;
        private readonly IConfigurationOptions _configurationService;

        public OutputFileNameProvider(ILogger<OutputFileNameProvider> logger, 
            IConfigurationOptions configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
        }

        public string GetAbsoluteFileName(string link)
        {
            _logger.LogInformation($"\tGetting output filename and filepath from link: {link}");

            var filename = default(string);

            var tlink = link.Trim();
            var si = tlink.LastIndexOf('/');
            if (si < 0) filename = tlink;
            else filename = tlink.Substring(si + 1);

            _logger.LogInformation($"\t\t{filename}");

            string filepath = Path.Combine(_configurationService.DownloadLocation, filename);
            _logger.LogInformation($"\t\t{filepath}");

            return filepath;
        }
    }
}
