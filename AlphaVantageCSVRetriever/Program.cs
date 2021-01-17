using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    class Program
    {
        internal const string UserConfigFile = "userConfig.json";
        internal const string AppConfigFile = "appConfig.json";

        private static readonly CancellationToken _cancellationToken = new();

        private static async Task Main(string[] args)
        {
            if( !CompositionRoot.Default.Initialized )
            {
                Console.WriteLine($"{nameof(CompositionRoot)} failed to initialize");
                Environment.ExitCode = -1;

                return;
            }

            // output any log events cached during configuration/startup
            CompositionRoot.Default.OutputCachedLogger();

            await CompositionRoot.Default.Host!.RunAsync( _cancellationToken );
        }
    }
}
