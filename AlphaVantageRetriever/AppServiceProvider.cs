using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.FppcFiling;
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

            builder.RegisterModule<FppcFilingDbAutofacModule>();

            builder.Register<FppcFilingConfiguration>( ( c, p ) =>
                 {
                     var retVal = new ConfigurationBuilder()
                         .SetBasePath( Environment.CurrentDirectory )
                         .AddUserSecrets<Program>()
                         .AddJsonFile( "configInfo.json" )
                         .Build()
                         .Get<FppcFilingConfiguration>();

                     return retVal;
                 } )
                .AsSelf()
                .SingleInstance();

            builder.Register<IJ4JLoggerConfiguration>(
                ( c, p ) => c.Resolve<FppcFilingConfiguration>().Logger );

            builder.Register<ILogger>( ( c, p ) =>
                 {
                     var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                     return new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .SetMinimumLevel( loggerConfig.MinLogLevel )
                         .WriteTo.Console( restrictedToMinimumLevel: loggerConfig.MinLogLevel )
                         .WriteTo.File(
                             path: J4JLoggingExtensions.DefineLocalAppDataLogPath( "log.txt" ),
                             restrictedToMinimumLevel: loggerConfig.MinLogLevel
                         )
                         .CreateLogger();
                 } )
                .SingleInstance();

            builder.RegisterGeneric( typeof( J4JLogger<> ) )
                .As( typeof( IJ4JLogger<> ) )
                .SingleInstance();

            return new AutofacServiceProvider( builder.Build() );
        }
    }
}
