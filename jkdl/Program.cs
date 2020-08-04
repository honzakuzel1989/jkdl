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
                else if (config.HasLink)
                {
                    await provider.Downloader.DownloadAsync(config.GetLink);
                }
                else
                {
                    var line = string.Empty;
                    while ((line = Console.ReadLine().Trim()) != string.Empty)
                    {
                        try
                        {
                            await provider.Downloader.DownloadAsync(line);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine(ex.ToString());
                        }
                    }
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
