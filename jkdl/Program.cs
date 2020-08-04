using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace jkdl
{
    internal class Program
    {
        protected Program()
        {
        }

        private static async Task Main(string[] args)
        {
            try
            {
                var config = new ConfigurationService(args);
                var provider = new ServiceCollectionWrapper(config);

                var cts = new CancellationTokenSource();
                _ = Task.Run(() => provider.Downloader.Run(cts.Token));

                var cmd = string.Empty;
                while (!cts.IsCancellationRequested)
                {
                    Console.Write("> ");
                    cmd = Console.ReadLine();
                    switch (cmd)
                    {
                        case "exit": 
                            cts.Cancel();
                            break;
                        case "link":
                            Console.Write("> ");
                            var link = Console.ReadLine();
                            provider.LinksCache.AddLink(link);
                            break;
                        case "file":
                            Console.Write("> ");
                            var filename = Console.ReadLine();
                            foreach(var flink in await provider.LinksProvider.GetLinks(new FileInfo(filename)))
                                provider.LinksCache.AddLink(flink);
                            break;
                    }
                }

                Console.WriteLine("Press [Enter] to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
