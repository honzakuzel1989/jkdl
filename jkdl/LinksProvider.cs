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

        public LinksProvider(ILogger<LinksProvider> logger)
        {
            _logger = logger;
        }

        public async Task<string[]> GetLinks(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                _logger.LogError($"File {fileInfo.FullName} does not exist!");
                return await Task.FromResult(Array.Empty<string>());
            }

            var lines = await File.ReadAllLinesAsync(fileInfo.FullName);
            return lines.Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();
        }
    }
}
