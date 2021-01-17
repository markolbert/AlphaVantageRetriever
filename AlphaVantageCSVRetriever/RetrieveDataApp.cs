using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly Configuration _config;
        private readonly DataRetriever _dataRetriever;
        private readonly IHost _host;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IJ4JLogger _logger;

        public RetrieveDataApp(
            IHost host,
            Configuration config,
            DataRetriever dataRetriever,
            IHostApplicationLifetime lifetime,
            IConfigurationUpdater<Configuration> configUpdater,
            IJ4JLogger logger
            )
        {
            _host = host;
            _config = config;
            _dataRetriever = dataRetriever;
            _lifetime = lifetime;

            _logger = logger;
            _logger.SetLoggedType( GetType() );

            if( configUpdater.Validate( _config ) ) 
                return;

            _logger.Fatal("Incomplete configuration, aborting");
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
                .OrderBy(d=>d.Ticker)
                .ToList();

            if( File.Exists( _config!.OutputFilePath ) )
                File.Delete( _config.OutputFilePath );

            await File.WriteAllTextAsync( _config.OutputFilePath, priceData.ToCsv(), cancellationToken );

            _logger.Information( "Ticker information retrieved" );

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
