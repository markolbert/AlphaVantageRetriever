using System;
using System.Collections.Generic;
using System.IO;
using AlphaVantageRetriever;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using ServiceStack.Text;

namespace AlphaVantage
{
    // thanx to https://medium.com/@mark.holdt/alphavantage-and-c-1d560e690387 for the inspiration for this!
    class Program
    {
        static void Main( string[] args )
        {
            var appConfig = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddJsonFile("configInfo.json")
                .Build();

            var configInfo = appConfig.Get<ConfigInfo>();

            var symbolInfo = File.ReadAllText( configInfo.PathToSecuritiesFile ).FromCsv<List<SymbolInfo>>();

            var symbol = "ARDM";

            var monthlyPrices = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&outputsize=full&apikey={configInfo.ApiKey}&datatype=csv"
                .GetStringFromUrl().FromCsv<List<AlphaVantageData>>();
        }
    }
}
