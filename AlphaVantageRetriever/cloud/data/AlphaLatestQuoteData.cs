using System;
using J4JSoftware.FileUtilities;

namespace J4JSoftware.AlphaVantageRetriever;

#pragma warning disable 8618
public class AlphaLatestQuoteData
{
    [CsvField("symbol")]
    public string Symbol { get; set; }
    [CsvField("open")]
    public decimal Open { get; set; }
    [CsvField("high")]
    public decimal High { get; set; }
    [CsvField("low")]
    public decimal Low { get; set; }
    [CsvField("price")]
    public decimal Price { get; set; }
    [CsvField("volume")]
    public decimal Volume { get; set; }
    [CsvField("latestDay")]
    public DateTime LatestDay { get; set; }
    [CsvField("previousClose")]
    public decimal PreviousClose { get; set; }
    [CsvField("change")]
    public decimal Change { get; set; }
    [CsvField("changePercent")]
    public decimal ChangePercent { get; set; }
}
