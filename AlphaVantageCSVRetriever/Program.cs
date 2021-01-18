using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    internal class Program
    {
        internal const string UserConfigFile = "userConfig.json";
        internal const string AppConfigFile = "appConfig.json";

        private static readonly CancellationToken _cancellationToken = new();

        private static async Task Main( string[] args )
        {
            if( !CompositionRoot.Default.Initialized )
            {
                Console.WriteLine( $"{nameof(CompositionRoot)} failed to initialize" );
                Environment.ExitCode = -1;

                return;
            }

            await CompositionRoot.Default.Host!.RunAsync( _cancellationToken );
        }
    }
}