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

                    return new FppcFilingContext( config );
                } )
                .AsSelf()
                .SingleInstance();
        }
    }
}
