using CommandLine;

namespace jkdl
{
    internal class ConfigurationService : IConfigurationService
    {
        public IConfigurationOptions Options { get; private set; }

        public ConfigurationService(IArgumentsWrapper argumentsWrapper)
        {
            Parser.Default.ParseArguments<ConfigurationOptions>(argumentsWrapper.Arguments)
                .WithParsed(opts => Options = opts);
        }
    }
}
