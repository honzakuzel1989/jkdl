using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace jkdl
{
    public class Downloader
    {
        private readonly ConcurrentDictionary<string, int> _progress = new ConcurrentDictionary<string, int>();

        private readonly TextWriter _stdout;
        private readonly TextWriter _stderr;

        public Downloader() : this(Console.Out, Console.Error)
        {

        }

        public Downloader(TextWriter stdout, TextWriter stderr)
        {
            _stdout = stdout;
            _stderr = stderr;
        }

        public async Task DownloadAsync(string link)
        {
            try
            {
                _stdout.WriteLine($"Downloading data from link: {link}");

                var filename = GetFileName(link);
                _progress[filename] = 0;

                if (!File.Exists(filename))
                {
                    using var client = new WebClient();
                    client.DownloadProgressChanged += (_, e) =>
                    {
                        if (e.ProgressPercentage > _progress[filename])
                        {
                            const int mult = 1_000_000;
                            const string suff = "MB";

                            _progress[filename] = e.ProgressPercentage;
                            _stdout.WriteLine($"\t{filename}\n\t\t{e.ProgressPercentage} [%]\t{e.BytesReceived / mult}/{e.TotalBytesToReceive / mult} [{suff}]");
                        }
                    };
                    await client.DownloadFileTaskAsync(link, filename);
                    _stdout.WriteLine($"File {filename} successfully downloaded");
                }
                else
                {
                    _stdout.WriteLine($"File {filename} already exists!");
                }
            }
            catch (Exception ex)
            {
                _stderr.WriteLine(ex.ToString());
            }
        }

        private string GetFileName(string link)
        {
            _stdout.WriteLine($"\tGetting filename from link: {link}");
            
            var filename = default(string);

            var tlink = link.Trim();
            var si = tlink.LastIndexOf('/');
            if (si < 0) filename = tlink;
            else filename = tlink.Substring(si + 1);

            _stdout.WriteLine($"\t\t{filename}");

            return filename;
        }
    }
}
