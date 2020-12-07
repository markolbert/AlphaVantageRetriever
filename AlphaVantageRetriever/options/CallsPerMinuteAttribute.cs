using System.ComponentModel.DataAnnotations;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class CallsPerMinuteAttribute : ValidationAttribute
    {
        public CallsPerMinuteAttribute()
            : base( "The calls per minute must be > 0" )
        {
        }

        protected override ValidationResult IsValid( object? value, ValidationContext validationContext )
        {
            if( value == null || ( value is int cpm && cpm <= 0 ) )
            {
                return new ValidationResult( FormatErrorMessage( validationContext.DisplayName ) );
            }

            return ValidationResult.Success!;
        }
    }
}