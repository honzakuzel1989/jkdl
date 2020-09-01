using System;
using System.Diagnostics;
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
                var cts = new CancellationTokenSource();

                var arguments = new ArgumentsWrapper(args);
                var configuration = new ConfigurationService(arguments);

                if (configuration.Options is null) Environment.Exit(42);

                var provider = new ServiceCollectionWrapper(configuration);

                if (!configuration.Options.Background)
                {
                    _ = Task.Run(async () => await provider.Downloader.Run(cts.Token));

                    await provider.ComandPrompt.Run(cts);

                    if (Debugger.IsAttached)
                    {
                        Console.Out.WriteLine("Press [Enter] to exit...");
                        Console.In.ReadLine();
                    }
                }
                else
                {
                    await provider.Downloader.Run(cts.Token);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
