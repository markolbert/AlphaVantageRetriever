using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    class Program
    {
        private static IServiceProvider _svcProvider;
        private static IJ4JLogger _logger;

        static void Main(string[] args)
        {
            ConfigureServices();

            _logger = _svcProvider.GetService<IJ4JLogger>();
            _logger.SetLoggedType<Program>();

            var retriever = _svcProvider.GetService<DataRetriever>();
            var priceData = retriever.GetPrices();

            var config = _svcProvider.GetService<Configuration>();

            if( File.Exists( config.OutputFilePath ) )
                File.Delete( config.OutputFilePath );

            File.WriteAllText( config.OutputFilePath, priceData.ToCsv() );
        }

        private static void ConfigureServices()
        {
            var builder = new ContainerBuilder();

            builder.Register((c, p) =>
                    new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        //.AddUserSecrets<Program>()
                        .AddJsonFile("appConfig.json")
                        .AddJsonFile("AlphaVantageAPI.key")
                        .Build())
                .As<IConfigurationRoot>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IConfigurationRoot>().Get<Configuration>())
                .AsSelf()
                .SingleInstance();

            builder.AddJ4JLogging( typeof(ConsoleChannel), typeof(FileChannel) );

            builder.RegisterType<DataRetriever>()
                .AsSelf()
                .SingleInstance();

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
