using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using J4JSoftware.FppcFiling;
using J4JSoftware.Logging;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace J4JSoftware.AlphaVantageRetriever
{
    [ Command(
        Name = "AlphaVantageRetriever",
        Description =
            "an app to retrieve stock ticker prices from AlphaVantage and export them in a format usable in FPPC filings"
    ) ]
    [ SingleCommandOption ]
    class Program
    {
        private static IJ4JLogger<Program> _logger;
        private static Timer _timer;

        [ Option( "-g|--get", "get data from AlphaVantage", CommandOptionType.NoValue ) ]
        internal bool Retrieve { get; set; }

        [Option( "-r|--replace", "replace existing data", CommandOptionType.NoValue )]
        internal bool ReplaceExistingData { get; set; }

        [ExportToFileOption ] 
        internal string PathToPriceFile { get; set; }

        [ UpdateSecurities ] 
        internal string PathToSecuritiesFile { get; set; }

        [Option( "-y|--year", "year (YYYY) to store", CommandOptionType.SingleValue ) ]
        [ ReportingYear ]
        internal int ReportingYear { get; set; }

        [ Option( "-c|--CallsPerMinute", "calls per minute to AlphaVantage site", CommandOptionType.SingleValue ) ]
        [ CallsPerMinute ]
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
                return RetrieveDataFromAlphaVantage();
            }

            if( !String.IsNullOrEmpty( PathToSecuritiesFile ) )
            {
                return UpdateSecurities();
            }

            if( !String.IsNullOrEmpty( PathToPriceFile ) )
            {
                return ExportDataToCSV();
            }

            // shouldn't get here; something weird happened
            _logger.Error( "Unexpected termination" );
            return -1;
        }

        private int RetrieveDataFromAlphaVantage()
        {
            // update from command line arguments, if specified
            if( ReportingYear > 0 )
                Configuration.ReportingYear = ReportingYear;

            if( CallsPerMinute > 0 )
                Configuration.CallsPerMinute = CallsPerMinute;

            // this AutoResetEvent is 'shared' by the calling method and the data retrieval method
            // and is used to indicate when all available SecurityInfo objects have been processed
            var jobDone = new AutoResetEvent( false );

            var interval = Convert.ToInt32( 60000 / Configuration.CallsPerMinute );

            var dataRetriever = AppServiceProvider.Instance.GetRequiredService<DataRetriever>();
            dataRetriever.Initialize(Configuration, ReplaceExistingData);

            var sb = new StringBuilder();
            
            sb.Append( $"Starting AlphaVantage extraction for {Configuration.ReportingYear} ({Configuration.CallsPerMinute} queries per minute, " );
            
            sb.Append( ReplaceExistingData
                ? "replace existing price data)..."
                : "skip securities with price data on file)..." );

            if( !ConfirmAction( sb.ToString() ) )
            {
                _logger.Information("Operation canceled");
                return -1;
            }

            // this creates the timer and starts it running
            _timer = new Timer( dataRetriever.ProcessNextSymbol, jobDone, 0, interval );

            // wait until the processing is done
            jobDone.WaitOne();

            _logger.Information( "Job finished" );

            return 0;
        }

        private int ExportDataToCSV()
        {
            if( PathToPriceFile != "@" )
                Configuration.PathToPriceFile = PathToPriceFile;

            if( String.IsNullOrEmpty( Configuration.PathToPriceFile ) )
                Configuration.PathToPriceFile = $"{Configuration.ReportingYear} {ExportToFileOptionAttribute.DefaultPathStub}";

            if( !MustBeValidFilePath.ValidatePath( Configuration.PathToPriceFile ) )
            {
                _logger.Error($"Export file '{Configuration.PathToPriceFile}' is invalid");

                return -1;
            }

            _logger.Information("Starting export of price data to CSV file...");

            var dbContext = AppServiceProvider.Instance.GetRequiredService<FppcFilingContext>();

            _logger.Information( "Retrieving market days..." );

            var marketDays = dbContext.HistoricalData
                .Select( hd => hd.Timestamp )
                .Distinct()
                .OrderByDescending( x => x )
                .ToList();

            var fracDays = marketDays.Count / 10;

            _logger.Information( "Retrieving CUSIPs..." );

            var headers = dbContext.Securities
                .Where(s=>s.Reportable  )
                .Select( s => s.Cusip )
                .Distinct()
                .OrderBy( x => x )
                .ToList();

            var prevHighs = new Dictionary<string, decimal>();

            try
            {
                var outputFile = File.CreateText( Configuration.PathToPriceFile );

                outputFile.Write( "Date" );
                headers.ForEach( h => outputFile.Write( $",{h}" ) );
                outputFile.WriteLine();

                var daysOutput = 0;
                var chunksOutput = 0;

                foreach( var marketDay in marketDays )
                {
                    var highs = dbContext.HistoricalData
                        .Where( hd => hd.Timestamp == marketDay )
                        .Select( hd => new { hd.SecurityInfo.Cusip, hd.High } )
                        .ToDictionary( x => x.Cusip, x => x.High );

                    outputFile.Write( marketDay.ToShortDateString() );

                    foreach( var curHeader in headers )
                    {
                        decimal toWrite = 0;

                        if( highs.ContainsKey( curHeader ) )
                        {
                            toWrite = highs[ curHeader ];

                            if( prevHighs.ContainsKey( curHeader ) )
                                prevHighs[ curHeader ] = highs[ curHeader ];
                            else
                                prevHighs.Add( curHeader, highs[ curHeader ] );
                        }
                        else
                        {
                            if( prevHighs.ContainsKey( curHeader ) )
                                toWrite = prevHighs[ curHeader ];
                        }

                        outputFile.Write( $", {toWrite}" );
                    }

                    outputFile.WriteLine();

                    daysOutput++;

                    if( daysOutput % fracDays == 0 )
                    {
                        chunksOutput++;
                        _logger.Information( $"{10 * chunksOutput}% written..." );
                    }

                    prevHighs = highs;
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

        private int UpdateSecurities()
        {
            if( PathToSecuritiesFile != "@" )
                Configuration.PathToSecuritiesFile = PathToSecuritiesFile;

            if( !File.Exists( Configuration.PathToSecuritiesFile ) )
            {
                _logger.Error( $"Securities file '{Configuration.PathToSecuritiesFile}' does not exist" );

                return -1;
            }

            var sb = new StringBuilder();

            sb.Append( "Starting securities update (" );
            sb.Append( ReplaceExistingData ? "erase existing data)..." : "retain existing data)..." );

            if( !ConfirmAction( sb.ToString() ) )
            {
                _logger.Information( "Operation canceled" );
                return -1;
            }

            _logger.Information("Reading securities file...");

            var symbols = File.ReadAllText( Configuration.PathToSecuritiesFile )
                .FromCsv<List<ImportedSecurityInfo>>();

            // check to ensure all cusips defined
            if( symbols.Any( s => String.IsNullOrEmpty( s.Cusip ) ) )
            {
                _logger.Error( "There are blank or undefined CUSIP numbers in the file. Please correct them." );
                return -1;
            }

            var dbContext = AppServiceProvider.Instance.GetRequiredService<FppcFilingContext>();

            if( ReplaceExistingData )
            {
                _logger.Information( "Removing existing data..." );

                dbContext.HistoricalData.FromSqlRaw( "truncate table HistoricalData" );
                dbContext.Securities.FromSqlRaw( "truncate table Securities" );
            }

            foreach( var curSymbol in symbols )
            {
                // see if security is in database
                var dbSecurity = dbContext.Securities
                    .Include( s => s.HistoricalData )
                    .FirstOrDefault( s => s.Cusip == curSymbol.Cusip );

                if( dbSecurity == null )
                {
                    dbSecurity = new FppcFiling.SecurityInfo
                    {
                        Cusip = curSymbol.Cusip
                    };

                    dbContext.Securities.Add( dbSecurity );

                    _logger.Information( $"Added {curSymbol.SecurityName}" );
                }
                else
                    _logger.Information( $"Updated {curSymbol.SecurityName}" );

                dbSecurity.SecurityName = curSymbol.SecurityName;
                dbSecurity.Type = curSymbol.Type;
                dbSecurity.Issuer = curSymbol.Issuer;
                dbSecurity.ClassSeries = curSymbol.ClassSeries;
                dbSecurity.Reportable = curSymbol.Reportable;
                dbSecurity.Ticker = curSymbol.Ticker;
            }

            dbContext.SaveChanges();

            _logger.Information( "Securities update completed" );

            return 0;
        }

        private bool ConfirmAction( string mesg )
        {
            Console.WriteLine($"{mesg}\n");

            Console.Write("Press Y or y to continue:");
            var key = Console.ReadKey().Key;
            Console.WriteLine();

            return key == ConsoleKey.Y;
        }
    }
}