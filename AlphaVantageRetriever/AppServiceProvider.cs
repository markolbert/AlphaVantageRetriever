using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever
{
    internal static class AppServiceProvider
    {
        private static AutofacServiceProvider _svcProvider;

        public static AutofacServiceProvider Instance => _svcProvider ??= ConfigureContainer();

        private static AutofacServiceProvider ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<AlphaVantageContext>()
                .AsSelf()
                .SingleInstance();

            var configRoot = new ConfigurationBuilder()
                .SetBasePath( Environment.CurrentDirectory )
                .AddUserSecrets<Program>()
                .AddJsonFile( "configInfo.json" )
                .Build();

            var channels = configRoot.GetSection( "Logger:Channels" ).Get<ChannelConfig>();

            builder.Register( c =>
                    new J4JLoggerConfiguration<ChannelConfig>
                    {
                        Channels = channels
                    } )
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register( c => configRoot.Get<AppConfiguration>() )
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterJ4JLogging();

            builder.RegisterType<DataRetriever>()
                .SingleInstance()
                .AsSelf();

            return new AutofacServiceProvider( builder.Build() );
        }
    }
}
