using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Configuration.CommandLine;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace J4JSoftware.AlphaVantageRetriever;

internal class Program
{
    internal const string UserConfigFile = "userConfig";
    internal const string AppConfigFile = "appConfig";

    private static readonly CancellationToken CancellationToken = new();

    private static async Task Main()
    {
        var builder = new HostBuilder()
                     .UseServiceProviderFactory( new AutofacServiceProviderFactory() )
                     .ConfigureHostConfiguration( SetupHost )
                     .ConfigureAppConfiguration( SetupApp )
                     .ConfigureContainer<ContainerBuilder>( SetupDependencyInjection );

        var host = builder.Build();

        var config = host.Services.GetRequiredService<Configuration>();

        if( config.VerboseLogging )
        {
            var loggerFactory = host.Services.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger<Program>();
            CommandLineLoggerFactory.Default.Dump( logger );
        }

        await host.RunAsync( CancellationToken );
    }

    private static void SetupHost( IConfigurationBuilder configBuilder )
    {
        configBuilder.AddEnvironmentVariables( "ALPHA_" );

        SetupCommandLineProcessing( configBuilder );
    }

    private static void SetupApp( HostBuilderContext hbc, IConfigurationBuilder configBuilder )
    {
        var env = hbc.HostingEnvironment;

        // link to application config files
        var dir = Environment.CurrentDirectory;
        configBuilder.AddJsonFile( Path.Combine( dir, $"{AppConfigFile}.json" ), true )
                     .AddJsonFile( Path.Combine( dir, $"{AppConfigFile}.{env.EnvironmentName}.json" ), true );

        dir = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                            "J4JSoftware",
                            nameof( AlphaVantageRetriever ) );

        configBuilder.AddJsonFile( Path.Combine( dir, $"{UserConfigFile}.json" ), true )
                     .AddJsonFile( Path.Combine( dir, $"{UserConfigFile}.{env.EnvironmentName}.json" ), true );

        if( env.IsDevelopment() && env.ApplicationName is { Length: > 0 } )
        {
            var appAssembly = Assembly.Load( new AssemblyName( env.ApplicationName ) );
            configBuilder.AddUserSecrets( appAssembly, true );
        }

        configBuilder.AddEnvironmentVariables();

        SetupCommandLineProcessing( configBuilder );
    }

    private static void SetupCommandLineProcessing( IConfigurationBuilder configBuilder )
    {
        CommandLineLoggerFactory.Default.EnableLogging = true;

        var optionBuilder = new J4JCommandLineBuilder( StringComparison.OrdinalIgnoreCase );

        // some command line parameters get bound to the UserConfiguration object so they
        // can be persisted
        optionBuilder.Bind<UserConfiguration, int>( x => x.ApiLimit.MaxRequests, "c" )!
               .SetDefaultValue( 25 )
               .SetDescription( "calls per interval" )
               .SetStyle( OptionStyle.SingleValued );

        optionBuilder.Bind<UserConfiguration, LimitInterval>( x => x.ApiLimit.Interval, "i" )!
               .SetDefaultValue( LimitInterval.Day )
               .SetDescription( "measurement interval" )
               .SetStyle( OptionStyle.SingleValued );

        // everything else gets bound to the TransientConfiguration object, which is not
        // persisted, although if an ApiKey is defined on the command line, and it's validated,
        // it will be used to update the UserConfiguration EncryptedApiKey property
        optionBuilder.Bind<Configuration, string?>( x => x.OutputFilePath, "o" )!
               .SetDescription( "output file" )
               .SetDefaultValue( "AlphaVantage" )
               .SetStyle( OptionStyle.SingleValued );

        optionBuilder.Bind<Configuration, OutputFormat>( x => x.OutputFormat, "s" )!
               .SetDescription( "output style" )
               .SetDefaultValue( OutputFormat.Csv )
               .SetStyle( OptionStyle.SingleValued );

        optionBuilder.Bind<Configuration, List<string>>( x => x.Tickers, "t" )!
               .SetDescription( "ticker symbols" )
               .SetStyle( OptionStyle.Collection );

        optionBuilder.Bind<Configuration, List<AlphaVantageData>>( x => x.DataToRetrieve, "d" )!
               .SetDescription( "data to be retrieved" )
               .SetStyle( OptionStyle.Collection );

        optionBuilder.Bind<Configuration, string?>( x => x.ApiKey, "k" )!
               .SetDescription( "API key" )
               .SetStyle( OptionStyle.SingleValued );

        optionBuilder.Bind<Configuration, bool>( x => x.VerboseLogging, "v" )!
               .SetDescription( "API key" )
               .SetStyle( OptionStyle.Switch );

        ILexicalElements? tokens = null;
        configBuilder.AddJ4JCommandLine( optionBuilder, ref tokens );
    }

    private static void SetupDependencyInjection( HostBuilderContext hbc, ContainerBuilder builder )
    {
        Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

        builder.Register( _ => new LoggerFactory().AddSerilog( Log.Logger ) )
               .AsImplementedInterfaces();

        builder.Register( _ =>
                {
                    var retVal = hbc.Configuration.Get<Configuration>();
                    retVal ??= new Configuration();

                    return retVal;
                } )
               .AsSelf()
               .SingleInstance();

        builder.RegisterType<DataRetriever>()
               .AsImplementedInterfaces();

        var dpProvider = DataProtectionProvider.Create( new DirectoryInfo(
                                                            Path.Combine(
                                                                Environment.GetFolderPath(
                                                                    Environment.SpecialFolder.LocalApplicationData ),
                                                                nameof( AlphaVantageRetriever ),
                                                                "DataProtection-Keys" ) ) );

        builder.Register( _ => dpProvider.CreateProtector( "apikey" ) )
               .AsImplementedInterfaces()
               .SingleInstance();

        builder.RegisterTypes( typeof( IParseAlpha ).Assembly.GetTypes() )
               .Where( t => !t.IsAbstract && t.GetInterface( nameof( IParseAlpha ) ) != null )
               .AsImplementedInterfaces()
               .SingleInstance();

        builder.RegisterType<AlphaVantageConnector>()
               .AsImplementedInterfaces()
               .SingleInstance();
    }
}
