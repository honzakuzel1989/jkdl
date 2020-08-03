using System;
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

                if (config.HasFilename)
                {
                    await provider.Downloader.DownloadAsync(config.GetFilename);
                }

                if (config.HasLink)
                {
                    await provider.Downloader.DownloadAsync(config.GetLink);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
