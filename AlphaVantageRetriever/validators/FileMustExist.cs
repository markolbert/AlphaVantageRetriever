using System.ComponentModel.DataAnnotations;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class FileMustExist : IOptionValidator
    {
        public ValidationResult GetValidationResult( CommandOption option, ValidationContext context )
        {
            if( !option.HasValue() || ( option.Value() == null ) ) return ValidationResult.Success!;

            if( !File.Exists(option.Value()) )
            {
                return new ValidationResult( $"The path '{option.Value()}' is invalid" );
            }

            return ValidationResult.Success!;
        }
    }
}