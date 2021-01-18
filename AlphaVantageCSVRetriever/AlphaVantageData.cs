using System;

#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class AlphaVantageData
    {
        public string Symbol { get; set; }
        public DateTime LatestDay { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
    }
}