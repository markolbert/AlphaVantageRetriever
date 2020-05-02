using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using J4JSoftware.Logging;
using ServiceStack;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class DataRetriever
    {
        private readonly object _lockObject = new object();
        private readonly IJ4JLogger _logger;
        private readonly Configuration _config;

        private static Timer _timer;
        private int _index = 0;

        public DataRetriever( 
            Configuration config,
            IJ4JLoggerFactory loggerFactory )
        {
            _config = config ?? throw new NullReferenceException( nameof(config) );

            _logger = loggerFactory?.CreateLogger(this.GetType()) 
                     ?? throw new NullReferenceException( nameof(loggerFactory) );
        }

        public void GetPrices()
        {
            _logger.Information("Initializing price retriever...");

            _config.Prices.Clear();

            // this AutoResetEvent is 'shared' by the calling method and the data retrieval method
            // and is used to indicate when all available SecurityInfo objects have been processed
            var jobDone = new AutoResetEvent(false);

            var interval = Convert.ToInt32( 60000 / _config.CallsPerMinute );
            
            // this creates the timer and starts it running
            _timer = new Timer(ProcessNextSymbol, jobDone, 0, interval);

            // wait until the processing is done
            jobDone.WaitOne();

            _logger.Information( "Price retrieval complete" );
        }

        public void ProcessNextSymbol( object stateInfo )
        {
            // wrap the call to the actual processing method in a monitor to
            // prevent multiple simultaneous accesses to the DbContext object
            if( Monitor.TryEnter( _lockObject ) )
            {
                try
                {
                    // check to ensure we were given the AutoResetEvent we're expecting
                    var jobDone = stateInfo as AutoResetEvent;

                    if( jobDone == null )
                        _logger.Error( $"Argument is not a {nameof( AutoResetEvent )}" );
                    else Process( jobDone );
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            }
        }

        protected void Process( AutoResetEvent jobDone )
        {
            var ticker = _config.Tickers[ _index ];

            // retrieve historical price data
            var url =
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={_config.ApiKey}&datatype=csv";
            var rawText = url.GetStringFromUrl();

            try
            {
                var rawData = rawText.FromCsv<AlphaVantageData>();

                var data = new Configuration.DailyPrice()
                {
                    Close = rawData.Price,
                    High = rawData.High,
                    Low = rawData.Low,
                    Volume = rawData.Volume,
                    Date = rawData.LatestDay
                };

                _config.Prices.Add( data );
            }
            catch(Exception e)
            {
                _logger.Error( $"Exception for {_config.Tickers[_index]}: {e.Message}" );
                _index++;
                
                return;
            }

            _logger.Information<string>( "{0} retrieved", _config.Tickers[_index] );

            _index++;

            if (_index >= _config.Tickers.Count)
                jobDone.Set();
        }
    }
}
