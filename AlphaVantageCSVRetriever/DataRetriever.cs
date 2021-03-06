﻿using System;
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
        private enum CurrentRetrieval
        {
            Prices,
            Name
        }

        private readonly object _lockObject = new object();
        private readonly IJ4JLogger _logger;
        private readonly Configuration _config;

        private readonly Dictionary<string, DailyPrice> _retrieved =
            new Dictionary<string, DailyPrice>( StringComparer.OrdinalIgnoreCase );

        private Timer? _timer;
        private int _index = 0;
        private CurrentRetrieval _currentRetrieval;

        public DataRetriever( 
            Configuration config,
            IJ4JLogger logger )
        {
            _config = config ?? throw new NullReferenceException( nameof(config) );
            _logger = logger ?? throw new NullReferenceException( nameof(logger) );

            //_logger.SetLoggedType<DataRetriever>();
        }

        public List<DailyPrice> GetPrices()
        {
            _logger.Information("Initializing price retriever...");

            _retrieved.Clear();

            // this AutoResetEvent is 'shared' by the calling method and the data retrieval method
            // and is used to indicate when all available SecurityInfo objects have been processed
            var jobDone = new AutoResetEvent(false);

            var interval = Convert.ToInt32( 60000 / _config.CallsPerMinute );
            _currentRetrieval = CurrentRetrieval.Prices;
            
            // this creates the timer and starts it running
            _timer = new Timer(ProcessNextSymbol, jobDone, 0, interval);

            // wait until the processing is done
            jobDone.WaitOne();

            _logger.Information( "Price retrieval complete" );

            return _retrieved.Select( kvp => kvp.Value )
                .ToList();
        }

        public void ProcessNextSymbol( object? stateInfo )
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
                    else
                    {
                        switch( _currentRetrieval )
                        {
                            case CurrentRetrieval.Prices:
                                RetrievePriceData(jobDone);
                                
                                _currentRetrieval = CurrentRetrieval.Name;

                                break;

                            case CurrentRetrieval.Name:
                                RetrieveSymbolInfo(jobDone);

                                _currentRetrieval = CurrentRetrieval.Prices;
                                _index++;

                                break;
                        }

                        if (_index >= _config.Securities.Count)
                            jobDone.Set();
                    }
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            }
        }

        protected void RetrievePriceData( AutoResetEvent jobDone )
        {
            var security = _config.Securities[ _index ];

            // retrieve historical price data
            var url =
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={security.Ticker}&apikey={_config.ApiKey}&datatype=csv";
            var rawText = url.GetStringFromUrl();

            try
            {
                var rawData = rawText.FromCsv<AlphaVantageData>();

                var data = new DailyPrice()
                {
                    Ticker = security.Ticker,
                    MoneydanceAccount = security.Moneydance.Account,
                    MoneydanceSecurity = security.Moneydance.Security,
                    Close = rawData.Price,
                    High = rawData.High,
                    Low = rawData.Low,
                    Volume = rawData.Volume,
                    Date = rawData.LatestDay
                };

                if( _retrieved.ContainsKey( security.Ticker ) ) _retrieved[ security.Ticker ] = data;
                else _retrieved.Add( security.Ticker, data );
            }
            catch(Exception e)
            {
                _logger.Error( $"Exception for {security}: {e.Message}" );
                return;
            }

            _logger.Information<string>( "Price data retrieved for {0}", security.Ticker );
        }

        protected void RetrieveSymbolInfo(AutoResetEvent jobDone)
        {
            var security = _config.Securities[_index];

            // retrieve search results
            var url =
                $"https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={security.Ticker}&apikey={_config.ApiKey}&datatype=csv";
            var rawText = url.GetStringFromUrl();

            try
            {
                var rawData = rawText.FromCsv<List<SearchResults>>()
                    .FirstOrDefault( rd => rd.Symbol.Equals(security.Ticker, StringComparison.OrdinalIgnoreCase) );

                if( rawData != null && _retrieved.ContainsKey( security.Ticker ) )
                {
                    _retrieved[ security.Ticker ].Name = rawData.Name;
                    _retrieved[ security.Ticker ].NameMatchScore = rawData.MatchScore;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Exception for {security}: {e.Message}");
                return;
            }

            _logger.Information<string>("Symbol info retrieved for {0}", security.Ticker);
        }
    }
}
