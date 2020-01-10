using System.IO;
using Autofac;
using J4JSoftware.FppcFiling;
using Microsoft.EntityFrameworkCore;

namespace J4JSoftware.FppcFiling
{
    public class FppcFilingDbAutofacModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            base.Load( builder );

            builder.Register<FppcFilingContext>( ( c, p ) =>
                 {
                     var config = c.Resolve<FppcFilingConfiguration>();

                    // WARNING: REMOVE FROM PRODUCTION
                    File.Delete( config.DatabasePath );

                     var retVal = new FppcFilingContext( config );

                    // this ensure the database file used by the app reflects
                    // all the latest configuration information
                    retVal.Database.Migrate();

                     return retVal;

                 } )
                .AsSelf()
                .SingleInstance();
        }
    }
}
