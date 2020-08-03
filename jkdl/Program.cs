﻿using jkdl.Wrappers;
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
                var config = ConfigurationBuilderWrapper.Build(args);
                var provider = ServiceCollectionWrapper.Create();

                if (config.HasFilename())
                {
                    await provider.GetDownloader().DownloadAsync(config.GetFilename());
                }

                if (config.HasLink())
                {
                    await provider.GetDownloader().DownloadAsync(config.GetLink());
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
