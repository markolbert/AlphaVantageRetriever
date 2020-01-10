using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using J4JSoftware.FppcFiling;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace J4JSoftware.AlphaVantageRetriever
{
    // thanx to https://medium.com/@mark.holdt/alphavantage-and-c-1d560e690387 for the inspiration for this!
    class Program
    {
        private static FppcFilingConfiguration _appConfig;
        private static IJ4JLogger<Program> _logger;

        static void Main( string[] args )
        {
            _logger = AppServiceProvider.Instance.GetRequiredService<IJ4JLogger<Program>>();
            _appConfig = AppServiceProvider.Instance.GetRequiredService<FppcFilingConfiguration>();

            //var appConfig = new ConfigurationBuilder()
            //    .AddUserSecrets<Program>()
            //    .AddJsonFile("configInfo.json")
            //    .Build();

            //var configInfo = appConfig.Get<ConfigInfo>();

            var symbols = File.ReadAllText( _appConfig.PathToSecuritiesFile ).FromCsv<List<SymbolInfo>>();

            foreach( var curSymbol in symbols.Where(s=>s.Reportable) )
            {
                if(String.IsNullOrEmpty(curSymbol.Ticker))
                    _logger.Error<string>("No ticker for {0}",curSymbol.Issuer  );
                else
                {
                    var monthlyPrices = 
                        $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={curSymbol.Ticker}&outputsize=full&apikey={_appConfig.ApiKey}&datatype=csv"
                            .GetStringFromUrl().FromCsv<List<AlphaVantageData>>();

                    _logger.Information<string, int>( "{0}: {1} lines retrieved", curSymbol.Ticker, monthlyPrices.Count );
                }
            }
        }
    }
}
