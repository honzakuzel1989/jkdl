using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jkdl
{
    internal class Program
    {
        protected Program()
        {
        }

        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("You have to provide filename with links like parameter!");
                return 10;
            }

            var services = CreateServiceCollection();
            var provider = CreateServiceProvider(services);

            try
            {
                var downloader = provider.GetRequiredService<IFileDownloader>();
                await downloader.DownloadAsync(new FileInfo(args.First()));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 30;
            }

            return 0;
        }

        private static ServiceProvider CreateServiceProvider(ServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        private static ServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging(l => l.AddConsole());

            services.AddTransient<IFileDownloader, FileDownloader>();
            services.AddTransient<IFileNameProvider, FileNameProvider>();
            services.AddTransient<ILinksProvider, LinksProvider>();

            return services;
        }
    }
}
