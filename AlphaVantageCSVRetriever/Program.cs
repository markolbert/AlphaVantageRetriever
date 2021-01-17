using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    class Program
    {
        private static IServiceProvider _svcProvider;
        private static IJ4JLogger _logger;

        static void Main(string[] args)
        {
            ConfigureServices();

            _logger = _svcProvider.GetService<IJ4JLogger>()!;
            _logger.SetLoggedType<Program>();

            var retriever = _svcProvider.GetService<DataRetriever>();

            var priceData = retriever!.GetPrices()
                .OrderBy(d=>d.MoneydanceAccount)
                .ToList();

            var config = _svcProvider.GetService<Configuration>();

            if( File.Exists( config!.OutputFilePath ) )
                File.Delete( config.OutputFilePath );

            File.WriteAllText( config.OutputFilePath, priceData.ToCsv() );
        }

        private static void ConfigureServices()
        {
            var builder = new ContainerBuilder();

            var configRoot = new ConfigurationBuilder()
                .SetBasePath( Environment.CurrentDirectory )
                .AddJsonFile( "appConfig.json" )
                .AddJsonFile( "AlphaVantageAPI.key" )
                .Build();

            builder.Register(c => configRoot.Get<Configuration>())
                .AsSelf()
                .SingleInstance();

            var channelInfo = new ChannelInformation()
                .AddChannel<ConsoleConfig>( "Logger:Channels:Console" )
                .AddChannel<FileConfig>( "Logger:Channels:File" );

            builder.RegisterJ4JLogging<J4JLoggerConfiguration>(
                new ChannelFactory( configRoot, channelInfo, "Logger" ) );

            builder.RegisterType<DataRetriever>()
                .AsSelf()
                .SingleInstance();

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
