using System;

namespace J4JSoftware.AlphaVantageRetriever;

#pragma warning disable 8618
public class AlphaLatestQuoteData
{
    public string Symbol { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Price { get; set; }
    public decimal Volume { get; set; }
    public DateTime LatestDay { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
}
