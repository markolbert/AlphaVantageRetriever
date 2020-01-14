using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace J4JSoftware.AlphaVantageRetriever
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExportToFileOptionAttribute : Attribute, IMemberConvention
    {
        public const string DefaultPathStub = "Fitch Trust Pricing Data.csv";

        public void Apply( ConventionContext context, MemberInfo member )
        {
            if( member is PropertyInfo property )
            {
                var opt = context.Application.Option( "-x|--export", "export data from database to CSV file",
                    CommandOptionType.SingleOrNoValue );

                opt.Validators.Add( new MustBeValidFilePath() );

                context.Application.OnParsingComplete( x =>
                {
                    if( opt.HasValue() )
                        property.SetValue( context.ModelAccessor.GetModel(), opt.Value() ?? "@" );
                } );
            }
        }
    }
}