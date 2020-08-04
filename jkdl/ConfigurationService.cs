using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace jkdl
{
    internal class ConfigurationService : IConfigurationService
    {
        private static readonly string FILENAME = "filename";
        private static readonly string LINK = "link";
        private static readonly string OVERWRITE = "overwrite";

        private static readonly Dictionary<string, string> _switchMappings = new Dictionary<string, string>()
        {
            { "-f", FILENAME },
            { "-l", LINK },
            { "-o", OVERWRITE },
        };
        
        private readonly IConfigurationRoot _configurationRoot;

        public ConfigurationService(string[] args)
        {
            _configurationRoot = new ConfigurationBuilder()
                .AddCommandLine(args, _switchMappings)
                .Build();
        }

        public bool HasFilename => !string.IsNullOrWhiteSpace(_configurationRoot[FILENAME]);

        public FileInfo GetFilename => new FileInfo(_configurationRoot[FILENAME]);

        public bool HasLink => !string.IsNullOrWhiteSpace(_configurationRoot[LINK]);

        public string GetLink => _configurationRoot[LINK];

        public bool OverwriteResult => true;
        public int MaxNumberOfDownload => 5;
    }
}
