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

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class EncryptApiKeyApp : IHostedService
    {
        private readonly Configuration _config;
        private readonly IHost _host;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IJ4JLogger _logger;

        public EncryptApiKeyApp(
            IHost host,
            Configuration config,
            IHostApplicationLifetime lifetime,
            IConfigurationUpdater<Configuration> configUpdater,
            IJ4JLogger logger
            )
        {
            _host = host;
            _config = config;
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
            if( !_config.EncryptKey )
            {
                _lifetime.StopApplication();
                return;
            }

            if( string.IsNullOrEmpty( _config.ApiKey ) )
            {
                _logger.Error("No AlphaVantage API key was specified to encrypt");
                _lifetime.StopApplication();

                return;
            }

            var tempConfig = new Configuration { ApiKey = _config.ApiKey };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var serialized = JsonSerializer.Serialize( tempConfig, jsonOptions);

            await File.WriteAllTextAsync( 
                Path.Combine( CompositionRoot.Default.UserConfigurationFolder, Program.UserConfigFile ), 
                serialized,
                cancellationToken );

            _logger.Information( "API key updated" );

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
