using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace J4JSoftware.AlphaVantageRetriever
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdateSecuritiesAttribute : Attribute, IMemberConvention
    {
        public const string DefaultPath = "Securities Data.csv";

        public void Apply( ConventionContext context, MemberInfo member )
        {
            if( member is PropertyInfo property )
            {
                var opt = context.Application.Option( "-u|--update", $"Update securities in database from CSV file (default is '{DefaultPath}')",
                    CommandOptionType.SingleOrNoValue );

                opt.Validators.Add( new FileMustExist() );

                context.Application.OnParsingComplete( x =>
                {
                    if( opt.HasValue())
                        property.SetValue( context.ModelAccessor.GetModel(), opt.Value() ?? "@" );
                } );
            }
        }
    }
}