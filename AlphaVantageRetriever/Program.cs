using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using J4JSoftware.FppcFiling;
using J4JSoftware.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace J4JSoftware.AlphaVantageRetriever
{
    // thanx to https://medium.com/@mark.holdt/alphavantage-and-c-1d560e690387 for the inspiration for this!
    class Program
    {
        private static FppcFilingConfiguration _appConfig;
        private static IJ4JLogger<Program> _logger;
        private static Timer _timer;

        static void Main( string[] args )
        {
            _logger = AppServiceProvider.Instance.GetRequiredService<IJ4JLogger<Program>>();
            _appConfig = AppServiceProvider.Instance.GetRequiredService<FppcFilingConfiguration>();

            // this AutoResetEvent is 'shared' by the calling method and the data retrieval method
            // and is used to indicate when all available SymbolInfo objects have been processed
            var jobDone = new AutoResetEvent( false );

            var interval = Convert.ToInt32( 60000 / _appConfig.CallsPerMinute );
            var dataRetriever = AppServiceProvider.Instance.GetRequiredService<DataRetriever>();

            _logger.Information( "Job started" );

            // this creates the timer and starts it running
            _timer = new Timer( dataRetriever.ProcessNextSymbol, jobDone, 0, interval);

            // wait until the processing is done
            jobDone.WaitOne();

            // now delete any securities from the database that aren't in the symbols file
            _logger.Information( "Deleting unneeded securities from database" );

            dataRetriever.DeleteUnusedSecurities();

            _logger.Information( "Job finished" );
        }
    }
}
