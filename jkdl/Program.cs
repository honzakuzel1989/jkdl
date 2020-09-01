using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

                if (configuration.Options is null)
                {
                    WaitForReturn();
                    Exit();
                }

                var provider = new ServiceCollectionWrapper(configuration);

                if (!configuration.Options.Background)
                {
                    _ = Task.Run(async () => await provider.Downloader.Run(cts.Token));

                    await provider.ComandPrompt.Run(cts);

                    WaitForReturn();
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

        [DoesNotReturn]
        private static void WaitForReturn()
        {
            if (Debugger.IsAttached)
            {
                Console.Out.WriteLine("Press [Enter] to exit...");
                Console.In.ReadLine();
            }
        }

        [DoesNotReturn]
        private static void Exit()
        {
            Environment.Exit(42);
        }
    }
}
