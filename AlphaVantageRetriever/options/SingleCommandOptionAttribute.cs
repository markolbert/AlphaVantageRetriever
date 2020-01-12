using System;
using System.ComponentModel.DataAnnotations;

namespace J4JSoftware.AlphaVantageRetriever
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleCommandOptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if( value is Program program )
            {
                int cmdOptions = 0;

                cmdOptions += program.Retrieve ? 1 : 0;
                cmdOptions += program.Export != null ? 1 : 0;
                cmdOptions += !String.IsNullOrEmpty( program.PathToSecuritiesFile ) ? 1 : 0;

                switch( cmdOptions )
                {
                    case 0:
                        return new ValidationResult("You must specify one of -r|--retrieve, -x|--export or -u|--update");

                    case 1:
                        return ValidationResult.Success;

                    default:
                        return new ValidationResult( "You can only specify one of -r|--retrieve, -x|--export or -u|--update" );
                }
            }

            return ValidationResult.Success;
        }
    }
}