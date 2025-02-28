using System;
using J4JSoftware.FileUtilities;

#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever;

public class AlphaPriceData
{
    public string Symbol { get; set; } = null!;
    [CsvField("timestamp")]
    public DateTime Timestamp { get; set; }
    [CsvField("open")]
    public decimal Open { get; set; }
    [CsvField("high")]
    public decimal High { get; set; }
    [CsvField("low")]
    public decimal Low { get; set; }
    [CsvField("close")]
    public decimal Close { get; set; }
    [CsvField("volume")]
    public decimal Volume { get; set; }
}
