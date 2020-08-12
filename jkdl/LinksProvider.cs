using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jkdl
{
    internal class LinksProvider : ILinksProvider
    {
        private readonly ILogger<LinksProvider> _logger;
        private readonly INotificationService _notificationService;

        public LinksProvider(ILogger<LinksProvider> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<string[]> GetLinks(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                await _notificationService.ProccessError(_logger, $"File {fileInfo.FullName} does not exist!");
                return await Task.FromResult(Array.Empty<string>());
            }

            var lines = await File.ReadAllLinesAsync(fileInfo.FullName);
            return lines.Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();
        }
    }
}
