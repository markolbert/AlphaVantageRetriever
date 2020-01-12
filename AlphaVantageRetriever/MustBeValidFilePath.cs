using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class MustBeValidFilePath : IOptionValidator
    {
        public ValidationResult GetValidationResult( CommandOption option, ValidationContext context )
        {
            if( !option.HasValue()) return ValidationResult.Success;

            if( !ValidatePath( option.Value() ?? ExportToFileOptionAttribute.DefaultPath ) )
            {
                return new ValidationResult( $"The path '{option.Value()}' is invalid" );
            }

            return ValidationResult.Success;
        }
        
        private bool ValidatePath( string filePath )
        {
            // check to see if file exists
            if( File.Exists( filePath ) ) return true;

            // if it doesn't exist try to create it
            try
            {
                var temp = File.CreateText( filePath );
                temp.Close();

                // remove it since we were just testing if we could create it
                File.Delete(filePath);
            }
            catch( Exception e )
            {
                return false;
            }

            return true;
        }
    }
}