using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using J4JSoftware.AlphaVantageRetriever;
using J4JSoftware.Logging;
using ServiceStack;
using HistoricalData = J4JSoftware.AlphaVantageRetriever.HistoricalData;
using SecurityInfo = J4JSoftware.AlphaVantageRetriever.SecurityInfo;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class DataRetriever
    {
        private readonly object _lockObject = new object();
        private AppConfiguration _config;
        private List<SecurityInfo> _securities;
        private int _index = 0;
        private bool _replaceExistingPriceData;

        public DataRetriever( 
            AlphaVantageContext dbContext,
            IJ4JLogger<DataRetriever> logger )
        {
            DbContext = dbContext ?? throw new NullReferenceException( nameof(dbContext) );
            Logger = logger ?? throw new NullReferenceException( nameof(logger) );
        }

        protected IJ4JLogger<DataRetriever> Logger { get; }
        protected AlphaVantageContext DbContext { get; }

        protected string ApiKey => _config?.ApiKey ?? "";

        public int ReportingYear => _config?.ReportingYear ?? -1;

        public void Initialize( AppConfiguration config, bool replaceExistingPriceData )
        {
            _config = config ?? throw new NullReferenceException( nameof(config) );

            _securities = DbContext.Securities.ToList();
            _index = 0;
            _replaceExistingPriceData = replaceExistingPriceData;
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
                        Logger.Error( $"Argument is not a {nameof( AutoResetEvent )}" );
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
            var mesg = new StringBuilder();
            SecurityInfo dbSecurity = null;

            // scan through all the SecurityInfo objects looking for the next one that
            // is reportable, has a ticker and hasn't already had its data retrieved
            while( true )
            {
                if( _index >= _securities.Count )
                {
                    jobDone.Set();
                    return;
                }

                // the StringBuilder instance 'mesg', in addition to holding any error
                // messages to report, is also the flag we use to tell that we've found
                // a SecurityInfo to process. that's done by checking to see if there are
                // no messages.
                mesg.Clear();

                dbSecurity = _securities[ _index ];

                if( !dbSecurity.Reportable )
                    mesg.Append( "not reportable" );

                bool emptyTicker = String.IsNullOrEmpty( dbSecurity.Ticker );

                if( emptyTicker )
                {
                    if( mesg.Length > 0 ) mesg.Append( "; " );
                    mesg.Append( "has no ticker symbol" );
                }

                if( dbSecurity.RetrievedData && _replaceExistingPriceData )
                {
                    if( mesg.Length > 0 ) mesg.Append( "; " );
                    mesg.Append( "price data already retrieved" );
                }

                // if no message we've found a symbol to process
                if( mesg.Length == 0 ) break;

                mesg.Insert( 0, $"{dbSecurity.Issuer} " );
                Logger.Information( mesg.ToString() );

                _index++;
            }

            // retrieve historical price data
            var url =
                $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={dbSecurity.Ticker}&outputsize=full&apikey={ApiKey}&datatype=csv";
            var rawText = url.GetStringFromUrl();

            List<AlphaVantageData> priceData = null;

            try
            {
                priceData = rawText.FromCsv<List<AlphaVantageData>>();
            }
            catch(Exception e)
            {
                dbSecurity.ErrorMessage = e.Message + "; " + rawText;
                DbContext.SaveChanges();

                Logger.Error( $"Exception for {dbSecurity.Issuer}: {dbSecurity.ErrorMessage}" );
                _index++;
                
                return;
            }

            Logger.Information<string, int>( "{0}: {1} lines retrieved", dbSecurity.Ticker, priceData.Count );

            DbContext.HistoricalData.AddRange( priceData
                .Where( pd => pd.Timestamp.Year == ReportingYear )
                .Select( pd => new HistoricalData()
                {
                    Close = pd.Close,
                    High = pd.High,
                    Low = pd.Low,
                    Open = pd.Open,
                    Timestamp = pd.Timestamp,
                    SecurityInfo = dbSecurity,
                    Volume = pd.Volume
                } ) );

            dbSecurity.RetrievedData = true;

            DbContext.SaveChanges();

            _index++;
        }
    }
}
