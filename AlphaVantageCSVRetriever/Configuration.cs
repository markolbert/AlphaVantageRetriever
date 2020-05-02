using System;
using System.Collections.Generic;
using System.Text;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class Configuration
    {
        public class DailyPrice
        {
            public DateTime Date { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal Close { get; set; }
            public decimal Volume { get; set; }
        }

        public string OutputFilePath { get; set; }
        public string ApiKey { get; set; }
        public float CallsPerMinute { get; set; }
        public List<string> Tickers { get; set; }
        public DateTime PriceDate { get; set; }

        public List<DailyPrice> Prices { get; } = new List<DailyPrice>();
    }
}
