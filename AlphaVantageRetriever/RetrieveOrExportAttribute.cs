using System;
using System.ComponentModel.DataAnnotations;

namespace J4JSoftware.AlphaVantageRetriever
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RetrieveOrExportAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if( value is Program program )
            {
                if( program.Retrieve && (program.Export != null) )
                {
                    return new ValidationResult(
                        "You cannot specify both -r|--retrieve and -x|--export at the same time" );
                }

                if( !program.Retrieve && (program.Export == null) )
                {
                    return new ValidationResult("You must specify either -r|--retrieve or -x|--export" );
                }
            }

            return ValidationResult.Success;
        }
    }
}