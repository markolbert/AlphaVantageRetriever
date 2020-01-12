using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class CommandLineArgs
    {
        [ Option( ShortName = "r", LongName = "retrieve", ShowInHelpText = true, 
            Description = "Retrieve ticker information from AlphaVantage" ) ]
        public bool RetrieveData { get; set; }

        [ Option( ShortName = "e", LongName = "export", ShowInHelpText = true,
            Description = "Export ticker information from database" ) ]
        public bool ExportData { get; set; }
    }
}
