using System;

namespace J4JSoftware.AlphaVantageRetriever;

public record PriceEntry(
    string Ticker,
    DateTime Date,
    TimeFrame TimeSpan,
    decimal Volume,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close
)
{
    public PriceEntry(string ticker, AlphaPriceData apd, TimeFrame timeFrame )
        : this( ticker, apd.Timestamp, timeFrame, apd.Volume, apd.Open, apd.High, apd.Low, apd.Close )
    {
    }
}
