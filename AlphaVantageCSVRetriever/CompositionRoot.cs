using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using J4JSoftware.Configuration.CommandLine;
using J4JSoftware.ConsoleUtilities;
using J4JSoftware.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class CompositionRoot : J4JCompositionRoot<J4JLoggerConfiguration>
    {
        public static CompositionRoot Default { get; }

        static CompositionRoot()
        {
            Default = new CompositionRoot();
            Default.Initialize();
        }

        private CompositionRoot()
            : base( "J4JSoftware", "AlphaVantageRetriever" )
        {
            UseConsoleLifetime = true;

            var channelConfig = new ChannelConfigProvider( "Logger" )
                .AddChannel<ConsoleConfig>( "Channels:Console" )
                .AddChannel<FileConfig>( "Channels:File" );

            ConfigurationBasedLogging( channelConfig );
        }

        public DataRetriever GetDataRetriever()
        {
            return Host!.Services.GetRequiredService<DataRetriever>();
        }

        public Configuration GetConfiguration()
        {
            return Host!.Services.GetRequiredService<Configuration>();
        }

        protected override void SetupConfigurationEnvironment( IConfigurationBuilder builder )
        {
            base.SetupConfigurationEnvironment( builder );

            var options = new OptionCollection( CommandLineStyle.Linux, loggerFactory: () => CachedLogger );

            options.Bind<Configuration, string>( c => c.OutputFilePath, "p" )!
                .SetDescription( "path to the output file" );

            options.Bind<Configuration, float>( c => c.CallsPerMinute, "c" )!
                .SetDefaultValue( 4.5F )
                .SetDescription( "calls per minute (float)" );

            options.Bind<Configuration, List<string>>( c => c.Tickers, "t" )!
                .SetDescription( "ticker symbols, separated by spaces" );

            options.Bind<Configuration, string>( c => c.APIKey, "k" )!
                .SetDescription( "AlphaVantage API key" );

            options.Bind<Configuration, bool>( c => c.EncryptKey, "e" )!
                .SetDescription( "Encrypt an API key" );

            builder.SetBasePath( Environment.CurrentDirectory )
                .AddJsonFile( Program.AppConfigFile, false )
                .AddJsonFile( Path.Combine( UserConfigurationFolder, Program.UserConfigFile ), true )
                .AddUserSecrets<Program>()
                .AddJ4JCommandLine( options );
        }

        protected override void SetupDependencyInjection( HostBuilderContext hbc, ContainerBuilder builder )
        {
            base.SetupDependencyInjection( hbc, builder );

            builder.Register( c => hbc.Configuration.Get<Configuration>() )
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<DataRetriever>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ConfigurationUpdater<Configuration>>()
                .OnActivating( x => { x.Instance.Property( c => c.APIKey, new ApiKeyUpdater( CachedLogger ) ); } )
                .Named<IConfigurationUpdater>( EncryptApiKeyApp.AutofacKey )
                .SingleInstance();

            builder.RegisterType<ConfigurationUpdater<Configuration>>()
                .OnActivating( x =>
                {
                    x.Instance.Property( c => c.Tickers, new TickerUpdater( CachedLogger ) );
                    x.Instance.Property( c => c.CallsPerMinute, new CallsPerMinuteUpdater( CachedLogger ) );
                    x.Instance.Property( c => c.OutputFilePath, new OutputFileUpdater( CachedLogger ) );
                } )
                .Named<IConfigurationUpdater>( RetrieveDataApp.AutofacKey )
                .SingleInstance();
        }

        protected override void SetupServices( HostBuilderContext hbc, IServiceCollection services )
        {
            base.SetupServices( hbc, services );

            var config = hbc.Configuration.Get<Configuration>();

            if( config!.EncryptKey )
                services.AddHostedService<EncryptApiKeyApp>();
            else
                services.AddHostedService<RetrieveDataApp>();
        }
    }
}