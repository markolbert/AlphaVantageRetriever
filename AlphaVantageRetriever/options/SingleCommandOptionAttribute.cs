using System;
using System.ComponentModel.DataAnnotations;

namespace J4JSoftware.AlphaVantageRetriever
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleCommandOptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid( object? value, ValidationContext validationContext )
        {
            if( value is Program program )
            {
                int cmdOptions = 0;

                cmdOptions += program.Retrieve ? 1 : 0;
                cmdOptions += program.PathToPriceFile != null ? 1 : 0;
                cmdOptions += !String.IsNullOrEmpty( program.PathToSecuritiesFile ) ? 1 : 0;

                string mesg = string.Empty;

                switch( cmdOptions )
                {
                    case 0:
                        mesg = "You must specify one of -g|--get, -x|--export or -u|--update";
                        //return new ValidationResult( "You must specify one of -g|--get, -x|--export or -u|--update" );
                        break;

                    case 1:
                        return ValidationResult.Success!;

                    default:
                        mesg = "You can only specify one of -g|--get, -x|--export or -u|--update";
                        //return new ValidationResult( "You can only specify one of -g|--get, -x|--export or -u|--update" );
                        break;
                }

                return new ValidationResult(mesg);
            }

            return ValidationResult.Success!;
        }
    }
}