using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.AlphaVantageRetriever;
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

            builder.Register<AlphaVantageContext>( ( c, p ) =>
                {
                    var config = c.Resolve<AppConfiguration>();

                    return new AlphaVantageContext( config );
                } )
                .AsSelf()
                .SingleInstance();

            builder.Register<AppConfiguration>( ( c, p ) =>
                 {
                     var retVal = new ConfigurationBuilder()
                         .SetBasePath( Environment.CurrentDirectory )
                         .AddUserSecrets<Program>()
                         .AddJsonFile( "configInfo.json" )
                         .Build()
                         .Get<AppConfiguration>();

                     return retVal;
                 } )
                .AsSelf()
                .SingleInstance();

            builder.Register<IJ4JLoggerConfiguration>(
                ( c, p ) => c.Resolve<AppConfiguration>().Logger );

            builder.Register<ILogger>( ( c, p ) =>
                 {
                     var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                     return new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .SetMinimumLevel( loggerConfig.MinLogLevel )
                         .WriteTo.Console( restrictedToMinimumLevel: loggerConfig.MinLogLevel )
                         .WriteTo.File(
                             path: J4JLoggingExtensions.DefineLocalAppDataLogPath( "log.txt", "J4JSoftware/AlphaVantageRetriever" ),
                             restrictedToMinimumLevel: loggerConfig.MinLogLevel
                         )
                         .CreateLogger();
                 } )
                .SingleInstance();

            builder.RegisterGeneric( typeof( J4JLogger<> ) )
                .As( typeof( IJ4JLogger<> ) )
                .SingleInstance();

            builder.RegisterType<DataRetriever>()
                .SingleInstance()
                .AsSelf();

            return new AutofacServiceProvider( builder.Build() );
        }
    }
}
