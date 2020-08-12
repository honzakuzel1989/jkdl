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
            services.AddSingleton<IDownloadClientsCache, DownloadClientsCache>();
            services.AddSingleton<ILinksCache, LinksCache>();
            services.AddSingleton<ICommandPrompt, CommandPrompt>();
            services.AddSingleton<ITextProvider, ConsoleTextProvider>();

            services.AddTransient<IOutputFileNameProvider, OutputFileNameProvider>();
            services.AddTransient<IFileDownloader, FileDownloader>();
            services.AddTransient<ILinksProvider, LinksProvider>();
            services.AddTransient<IWebClientFactory, WebClientFactory>();
            services.AddTransient<IDownloadProgressProvider, DownloadProgressProvider>();
            services.AddTransient<IDownloadProgressMonitor, DownloadProgressMonitor>();
            services.AddTransient<INotificationService, NotificationService>();

            _serviceProvider = services.BuildServiceProvider();
        }

        internal ICommandPrompt ComandPrompt => _serviceProvider.GetRequiredService<ICommandPrompt>();
        internal IFileDownloader Downloader => _serviceProvider.GetRequiredService<IFileDownloader>();
    }
}
