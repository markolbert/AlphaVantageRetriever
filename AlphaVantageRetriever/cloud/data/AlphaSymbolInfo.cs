using System;
using System.Collections.Generic;
using J4JSoftware.FileUtilities;

namespace J4JSoftware.AlphaVantageRetriever;

public class AlphaSymbolInfo
{
    public sealed class SymbolEqualityComparer : IEqualityComparer<AlphaSymbolInfo>
    {
        public bool Equals( AlphaSymbolInfo? x, AlphaSymbolInfo? y )
        {
            if( ReferenceEquals( x, y ) )
                return true;
            if( x is null )
                return false;
            if( y is null )
                return false;

            return x.GetType() == y.GetType()
             && string.Equals( x.Symbol, y.Symbol, StringComparison.OrdinalIgnoreCase );
        }

        public int GetHashCode( AlphaSymbolInfo obj ) => StringComparer.OrdinalIgnoreCase.GetHashCode( obj.Symbol );
    }

    [CsvField("symbol")]
    public string Symbol { get; set; } = null!;
    [CsvField("name")]
    public string Name { get; set; } = null!;
    [CsvField("region")]
    public string Region { get; set; } = null!;
    [CsvField("marketOpen")]
    public TimeSpan MarketOpen { get; set; }
    [ CsvField( "marketClose" ) ]
    public TimeSpan MarketClose { get; set; }
    [CsvField("timezone")]
    public string TimeZone { get; set; } = null!;
    [CsvField("currency")]
    public string Currency { get; set; } = null!;
    [CsvField("matchScore")]
    public decimal MatchScore { get; set; }
}
