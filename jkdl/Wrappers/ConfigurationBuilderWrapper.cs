using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace jkdl.Wrappers
{
    internal static class ConfigurationBuilderWrapper
    {
        private static readonly string FILENAME = "filename";
        private static readonly string LINK = "link";

        private static readonly Dictionary<string, string> _switchMappings = new Dictionary<string, string>()
        {
            { "-f", FILENAME },
            { "-l", LINK },
        };

        internal static IConfigurationRoot Build(string[] args)
            => new ConfigurationBuilder().AddCommandLine(args, _switchMappings).Build();

        internal static bool HasFilename(this IConfigurationRoot configurationRoot)
            => !string.IsNullOrWhiteSpace(configurationRoot[FILENAME]);

        internal static FileInfo GetFilename(this IConfigurationRoot configurationRoot)
            => new FileInfo(configurationRoot[FILENAME]);

        internal static bool HasLink(this IConfigurationRoot configurationRoot)
            => !string.IsNullOrWhiteSpace(configurationRoot[LINK]);

        internal static string GetLink(this IConfigurationRoot configurationRoot)
            => configurationRoot[LINK];
    }
}
