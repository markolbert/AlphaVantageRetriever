using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using J4JSoftware.ConsoleUtilities;
using J4JSoftware.Logging;
using Microsoft.Extensions.Hosting;
using ServiceStack;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class RetrieveDataApp : IHostedService
    {
        internal const string AutofacKey = "RetrieveData";

        private readonly Configuration _config;
        private readonly DataRetriever _dataRetriever;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IJ4JLogger _logger;

        public RetrieveDataApp(
            Configuration config,
            DataRetriever dataRetriever,
            IHostApplicationLifetime lifetime,
            IIndex<string, IConfigurationUpdater> configUpdaters,
            IJ4JLogger logger
        )
        {
            _config = config;
            _dataRetriever = dataRetriever;
            _lifetime = lifetime;

            _logger = logger;
            _logger.SetLoggedType( GetType() );

            if( configUpdaters.TryGetValue( AutofacKey, out var configUpdater )
                && configUpdater.Update( _config ) )
                return;

            _logger.Fatal( "Incomplete configuration, aborting" );
            _lifetime.StopApplication();
        }

        public async Task StartAsync( CancellationToken cancellationToken )
        {
            if( _config.EncryptKey )
            {
                _lifetime.StopApplication();
                return;
            }

            var priceData = _dataRetriever!.GetPrices()
                .OrderBy( d => d.Ticker )
                .ToList();

            if( File.Exists( _config!.OutputFilePath ) )
                File.Delete( _config.OutputFilePath );

            await File.WriteAllTextAsync( _config.OutputFilePath, priceData.ToCsv(), cancellationToken );

            _logger.Information<string>( "Ticker information written to {0}", _config.OutputFilePath );

            _lifetime.StopApplication();
        }

#pragma warning disable 1998
        public async Task StopAsync( CancellationToken cancellationToken )
#pragma warning restore 1998
        {
            _lifetime.StopApplication();
        }
    }
}