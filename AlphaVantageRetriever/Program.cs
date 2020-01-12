using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using J4JSoftware.FppcFiling;
using J4JSoftware.Logging;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Text;

namespace J4JSoftware.AlphaVantageRetriever
{
    // thanx to https://medium.com/@mark.holdt/alphavantage-and-c-1d560e690387 for the inspiration for this!
    [Command(
        Name="AlphaVantageRetriever", 
        Description = "an app to retrieve stock ticker prices from AlphaVantage and export them in a format usable in FPPC filings"
        )]
    [RetrieveOrExport]
    class Program
    {
        private static IJ4JLogger<Program> _logger;
        private static Timer _timer;

        [Option("-r|--retrieve", "retrieve data from AlphaVantage",CommandOptionType.NoValue)]
        internal bool Retrieve { get; set; }

        [ExportToFileOption]
        internal string Export { get; set; }

        [Option("-s|--securities","path to CSV file of securities to import", CommandOptionType.SingleValue)]
        [FileExists]
        internal string PathToSecuritiesFile { get; set; }

        [ Option( "-y|--year", "year (YYYY) to store", CommandOptionType.SingleValue ) ]
        [ReportingYear]
        internal int ReportingYear { get; set; }

        [ Option( "-c|--CallsPerMinute", "calls per minute to AlphaVantage site", CommandOptionType.SingleValue ) ]
        [CallsPerMinute]
        internal float CallsPerMinute { get; set; }

        private FppcFilingConfiguration Configuration { get; set; }

        static Task<int> Main( string[] args ) => CommandLineApplication.ExecuteAsync<Program>( args );

        private async Task<int> OnExecuteAsync( 
            CommandLineApplication app,
            CancellationToken cancellationToken = default 
            )
        {
            _logger = AppServiceProvider.Instance.GetRequiredService<IJ4JLogger<Program>>();
            Configuration = AppServiceProvider.Instance.GetRequiredService<FppcFilingConfiguration>();

            if( Retrieve )
            {
                // update from command line arguments, if specified
                if( ReportingYear > 0 ) Configuration.ReportingYear = ReportingYear;

                if( CallsPerMinute > 0 ) Configuration.CallsPerMinute = CallsPerMinute;
                
                if( !String.IsNullOrEmpty( PathToSecuritiesFile ) )
                    Configuration.PathToSecuritiesFile = PathToSecuritiesFile;

                return RetrieveDataFromAlphaVantage();
            }

            if( !String.IsNullOrEmpty(Export) )
            {
                return ExportDataToCSV();
            }

            // shouldn't get here; something weird happened
            _logger.Error( "Unexpected termination" );
            return -1;
        }

        private int RetrieveDataFromAlphaVantage()
        {
            // this AutoResetEvent is 'shared' by the calling method and the data retrieval method
            // and is used to indicate when all available SymbolInfo objects have been processed
            var jobDone = new AutoResetEvent( false );

            var interval = Convert.ToInt32( 60000 / Configuration.CallsPerMinute );
            var dataRetriever = AppServiceProvider.Instance.GetRequiredService<DataRetriever>();

            _logger.Information( "Job started" );

            // this creates the timer and starts it running
            _timer = new Timer( dataRetriever.ProcessNextSymbol, jobDone, 0, interval );

            // wait until the processing is done
            jobDone.WaitOne();

            // now delete any securities from the database that aren't in the symbols file
            _logger.Information( "Deleting unneeded securities from database" );

            dataRetriever.DeleteUnusedSecurities();

            _logger.Information( "Job finished" );

            return 0;
        }

        private int ExportDataToCSV()
        {
            var dbContext = AppServiceProvider.Instance.GetRequiredService<FppcFilingContext>();

            _logger.Information( "Retrieving market days..." );

            var marketDays = dbContext.HistoricalData
                .Select( hd => hd.Timestamp )
                .Distinct()
                .OrderBy( x => x )
                .ToList();

            var fracDays = marketDays.Count / 10;

            _logger.Information("Retrieving tickers...");

            var headers = dbContext.HistoricalData
                .Select(hd=>hd.SecurityInfo.Ticker)
                .Distinct()
                .OrderBy( x => x )
                .ToList();

            var prevDayHigh = headers.ToDictionary( x => x, x => 0.0M );

            try
            {
                var outputFile = File.CreateText( Export );

                outputFile.Write( "Date" );
                headers.ForEach( h => outputFile.Write( $",{h}" ) );
                outputFile.WriteLine();

                var daysOutput = 0;
                var chunksOutput = 0;

                foreach( var marketDay in marketDays )
                {
                    var dayHigh = dbContext.HistoricalData
                        .Where(hd=>hd.Timestamp == marketDay  )
                        .Select( hd => new { hd.SecurityInfo.Ticker, hd.High } )
                        .ToDictionary( x => x.Ticker, x => x.High );

                    outputFile.Write( marketDay.ToShortDateString() );

                    headers.ForEach( h =>
                    {
                        var high = dayHigh.ContainsKey( h ) ? dayHigh[ h ] : prevDayHigh[ h ];
                        outputFile.Write( $", {high}" );
                    } );

                    outputFile.WriteLine();

                    daysOutput++;

                    if( daysOutput % fracDays == 0 )
                    {
                        chunksOutput++;
                        _logger.Information( $"{10 * chunksOutput}% written..." );
                    }

                    prevDayHigh = dayHigh;
                }

                outputFile.Flush();
                outputFile.Close();
            }
            catch( Exception e )
            {
                _logger.Error( e.Message );
                return -1;
            }

            _logger.Information( "Export completed" );

            return 0;
        }
    }
}
