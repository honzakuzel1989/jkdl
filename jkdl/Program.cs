using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jkdl
{
    internal class Program
    {
        public Program()
        {
        }

        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("You have to provide filename with links like parameter!");
                return 10;
            }

            var filename = args.First();

            if (!File.Exists(filename))
            {
                Console.Error.WriteLine($"File {filename} does not exist!");
                return 20;
            }

            try
            {
                var downloader = new Downloader();

                var links = await File.ReadAllLinesAsync(filename);
                var tasks = links.Select(async link => await downloader.DownloadAsync(link));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 30;
            }

            return 0;
        }
    }
}
