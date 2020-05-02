using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoFacJ4JLogging;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace J4JSoftware.AlphaVantageRetriever
{
    internal static class AppServiceProvider
    {
        private static AutofacServiceProvider _svcProvider;

        public static AutofacServiceProvider Instance => _svcProvider ??= ConfigureContainer();

        private static AutofacServiceProvider ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register( ( c, p ) =>
                {
                    var config = c.Resolve<AppConfiguration>();

                    return new AlphaVantageContext( config );
                } )
                .AsSelf()
                .SingleInstance();

            builder.Register( ( c, p ) =>
                    new ConfigurationBuilder()
                        .SetBasePath( Environment.CurrentDirectory )
                        .AddUserSecrets<Program>()
                        .AddJsonFile( "configInfo.json" )
                        .Build() )
                .As<IConfigurationRoot>()
                .SingleInstance();

            builder.Register( c => c.Resolve<IConfigurationRoot>().Get<AppConfiguration>() )
                .AsSelf()
                .SingleInstance();

            builder.Register(
                    c =>
                    {
                        var configRoot = c.Resolve<IConfigurationRoot>();

                        var loggerBuilder = new J4JLoggerConfigurationRootBuilder();

                        loggerBuilder
                            .AddChannel<ConsoleChannel>()
                            .AddChannel<FileChannel>();

                        return loggerBuilder.Build<J4JLoggerConfiguration>( configRoot, "Logger" );
                    } )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.AddJ4JLogging();

            builder.RegisterType<DataRetriever>()
                .SingleInstance()
                .AsSelf();

            return new AutofacServiceProvider( builder.Build() );
        }
    }
}
