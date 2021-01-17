using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using J4JSoftware.Configuration.CommandLine;
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
            Default = new CompositionRoot()
            {
                CachedLoggerScope = CachedLoggerScope.SingleInstance,
                IncludeLastEvent = false,
                LoggingSectionKey = "Logger",
                UseConsoleLifetime = true
            };

            Default.ChannelInformation.AddChannel<ConsoleConfig>( "Logger:Channels:Console" )
                .AddChannel<FileConfig>( "Logger.Channels.File" );

            Default.Initialize();
        }

        private CompositionRoot()
            : base( "J4JSoftware", "AlphaVantageRetriever" )
        {
        }

        public DataRetriever GetDataRetriever() => Host!.Services.GetRequiredService<DataRetriever>();
        public Configuration GetConfiguration() => Host!.Services.GetRequiredService<Configuration>();

        protected override void SetupConfigurationEnvironment( IConfigurationBuilder builder )
        {
            base.SetupConfigurationEnvironment( builder );

            var options = new OptionCollection();

            options.Bind<Configuration, string>( c => c.OutputFilePath, "p" )!
                .SetDescription( "path to the output file" );

            options.Bind<Configuration, float>( c => c.CallsPerMinute, "c" )!
                .SetDefaultValue( 4.5F )
                .SetDescription( "calls per minute (float)" );

            options.Bind<Configuration, List<string>>( c => c.Tickers, "s" )!
                .SetDescription( "ticker symbols, separated by spaces" );

            options.Bind<Configuration, string>( c => c.ApiKey, "k" )!
                .SetDescription( "AlphaVantage API key" );

            options.Bind<Configuration, bool>( c => c.EncryptKey, "e" )!
                .SetDescription( "Encrypt an API key" );

            builder.SetBasePath( Environment.CurrentDirectory )
                .AddJsonFile( Program.AppConfigFile, false )
                .AddJsonFile( Path.Combine( UserConfigurationFolder, Program.UserConfigFile ), true )
                .AddUserSecrets<Program>()
                .AddJ4JCommandLine( options )
                .Build();
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
        }

        protected override void SetupServices( HostBuilderContext hbc, IServiceCollection services )
        {
            base.SetupServices( hbc, services );

            var config = hbc.Configuration.Get<Configuration>();

            if (config!.EncryptKey)
                services.AddHostedService<EncryptApiKeyApp>();
            else
                services.AddHostedService<RetrieveDataApp>();
        }
    }
}
