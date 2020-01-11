using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using J4JSoftware.FppcFiling;
using J4JSoftware.Logging;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class DataRetriever
    {
        private readonly object _lockObject = new object();

        public DataRetriever( 
            FppcFilingContext dbContext,
            FppcFilingConfiguration config, 
            IJ4JLogger<DataRetriever> logger )
        {
            DbContext = dbContext ?? throw new NullReferenceException( nameof(dbContext) );
            Logger = logger ?? throw new NullReferenceException( nameof(logger) );
            var temp = config ?? throw new NullReferenceException( nameof(config) );

            Symbols = File.ReadAllText( temp.PathToSecuritiesFile )
                .FromCsv<List<SymbolInfo>>();

            ReportingYear = temp.ReportingYear;
            ApiKey = temp.ApiKey;
        }

        protected IJ4JLogger<DataRetriever> Logger { get; }
        protected FppcFilingContext DbContext { get; }
        protected string ApiKey { get; }

        public List<SymbolInfo> Symbols { get; }
        public int Index { get; private set; }
        public int ReportingYear { get; }

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

        public void DeleteUnusedSecurities()
        {
            var toRemove = DbContext.Securities
                .Where( s => Symbols.Select( x => x.Ticker ).All( t => t != s.Ticker ) )
                .ToList();

            DbContext.RemoveRange(toRemove);

            DbContext.SaveChanges();
        }

        protected void Process( AutoResetEvent jobDone )
        {
            if( Index >= Symbols.Count )
            {
                jobDone.Set();
                return;
            }

            var mesg = new StringBuilder();
            SecurityInfo dbSecurity = null;

            // scan through all the SymbolInfo objects looking for the next one that
            // is reportable, has a ticker and hasn't already had its data retrieved
            while( true )
            {
                // the StringBuilder instance 'mesg', in addition to holding any error
                // messages to report, is also the flag we use to tell that we've found
                // a SymbolInfo to process. that's done by checking to see if there are
                // no messages.
                mesg.Clear();

                bool reportable = Symbols[ Index ].Reportable;

                if( !reportable )
                    mesg.Append( "not reportable" );

                bool emptyTicker = String.IsNullOrEmpty( Symbols[ Index ].Ticker );

                if( emptyTicker )
                {
                    if( mesg.Length > 0 ) mesg.Append( "; " );
                    mesg.Append( "has no ticker symbol" );
                }
                else
                {
                    // check ticker
                    var curSymbol = Symbols[ Index ];

                    // see if security is in database
                    dbSecurity = DbContext.Securities
                        .Include( s => s.HistoricalData )
                        .FirstOrDefault( s => s.Ticker == curSymbol.Ticker );

                    if( dbSecurity != null )
                    {
                        dbSecurity.ErrorMessage = null;

                        if( dbSecurity.RetrievedData )
                        {
                            if( reportable )
                            {
                                if( mesg.Length > 0 ) mesg.Append( "; " );
                                mesg.Append( "already retrieved data" );
                            }
                            else
                            {
                                if( mesg.Length > 0 ) mesg.Append( "; " );
                                mesg.Append( "retrieved data but no longer reportable; deleting retrieved data, removing security" );

                                DbContext.RemoveRange( dbSecurity.HistoricalData );
                                DbContext.Remove( dbSecurity );
                            }
                        }
                    }
                    else
                    {
                        if( reportable )
                        {
                            dbSecurity = new SecurityInfo
                            {
                                Ticker = curSymbol.Ticker,
                                Issuer = curSymbol.Issuer,
                                Category = curSymbol.Category,
                                Reportable = curSymbol.Reportable
                            };

                            DbContext.Securities.Add( dbSecurity );
                        }
                    }
                }

                DbContext.SaveChanges();

                // if no message we've found a symbol to process
                if( mesg.Length == 0 ) break;

                mesg.Insert( 0, $"{Symbols[ Index ].Issuer} " );
                Logger.Information( mesg.ToString() );

                Index++;

                // flag that we're done when we've exhausted the list of symbols
                if( Index >= Symbols.Count )
                {
                    jobDone.Set();
                    return;
                }
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
                Index++;
                
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

            Index++;
        }
    }
}
