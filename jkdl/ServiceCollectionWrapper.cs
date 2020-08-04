using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace jkdl
{
    internal class ServiceCollectionWrapper
    {
        public IServiceProvider _serviceProvider { get; }

        public ServiceCollectionWrapper(IConfigurationService config)
        {
            var services = new ServiceCollection();
            services.AddLogging(l => l.AddDebug());

            services.AddSingleton<IConfigurationService>(config);
            services.AddSingleton<IDownloadProgressCache, DownloadProgressCache>();
            services.AddSingleton<ILinksCache, LinksCache>();
            services.AddSingleton<ICommandPrompt, CommandPrompt>();

            services.AddTransient<IFileDownloader, FileDownloader>();
            services.AddTransient<IFileNameProvider, FileNameProvider>();
            services.AddTransient<ILinksProvider, LinksProvider>();
            services.AddTransient<IWebClientFactory, WebClientFactory>();
            services.AddTransient<IDownloadProgressProvider, DownloadProgressProvider>();

            _serviceProvider = services.BuildServiceProvider();
        }

        internal ICommandPrompt ComandPrompt => _serviceProvider.GetRequiredService<ICommandPrompt>();
    }
}
