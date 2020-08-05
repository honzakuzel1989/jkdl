﻿using System;
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

                await provider.ComandPrompt.RunAsync();

                Console.Out.WriteLine("Press [Enter] to exit...");
                Console.In.ReadLine();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
