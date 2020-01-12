using System;
using System.ComponentModel.DataAnnotations;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class ReportingYearAttribute : ValidationAttribute
    {
        public ReportingYearAttribute()
            : base( $"The reporting year must be between 2000 and {DateTime.Today.Year}" )
        {
        }

        protected override ValidationResult IsValid( object value, ValidationContext validationContext )
        {
            if( value == null || ( value is int repYear && ( repYear < 2000 || repYear > DateTime.Today.Year ) ) )
            {
                return new ValidationResult( FormatErrorMessage( validationContext.DisplayName ) );
            }

            return ValidationResult.Success;
        }
    }
}