using System.IO;
using System.Text.Json;
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
        internal const string AutofacKey = "EncryptKey";

        private readonly Configuration _config;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly IJ4JLogger _logger;

        public EncryptApiKeyApp(
            Configuration config,
            IHostApplicationLifetime lifetime,
            IIndex<string, IConfigurationUpdater> configUpdaters,
            IJ4JLogger logger
        )
        {
            _config = config;
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
            if( !_config.EncryptKey )
            {
                _lifetime.StopApplication();
                return;
            }

            if( string.IsNullOrEmpty( _config.APIKey ) )
            {
                _logger.Error( "No AlphaVantage API key was specified to encrypt" );
                _lifetime.StopApplication();

                return;
            }

            var tempConfig = new Configuration { APIKey = _config.APIKey };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            jsonOptions.Converters.Add( new EncryptedAPIKeyConverter() );
            var serialized = JsonSerializer.Serialize( tempConfig, jsonOptions );

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