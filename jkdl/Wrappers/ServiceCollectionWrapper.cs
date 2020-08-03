using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace jkdl.Wrappers
{
    internal static class ServiceCollectionWrapper
    {
        internal static IServiceProvider Create()
        {
            var services = new ServiceCollection();
            services.AddLogging(l => l.AddConsole());

            services.AddSingleton<IDownloadProgressCache, DownloadProgressCache>();

            services.AddTransient<IFileDownloader, FileDownloader>();
            services.AddTransient<IFileNameProvider, FileNameProvider>();
            services.AddTransient<ILinksProvider, LinksProvider>();
            services.AddTransient<IWebClientFactory, WebClientFactory>();
            services.AddTransient<IDownloadProgressProvider, DownloadProgressProvider>();

            return services.BuildServiceProvider();
        }

        internal static IFileDownloader GetDownloader(this IServiceProvider serviceProvider)
            => serviceProvider.GetRequiredService<IFileDownloader>();
    }
}
