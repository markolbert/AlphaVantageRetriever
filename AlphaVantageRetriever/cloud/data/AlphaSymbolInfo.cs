using System;
using System.Collections.Generic;

namespace J4JSoftware.AlphaVantageRetriever;

public record AlphaSymbolInfo( string Symbol, string Name, string Region, string Currency, decimal MatchScore)
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

            return x.GetType() == y.GetType() && string.Equals( x.Symbol, y.Symbol, StringComparison.OrdinalIgnoreCase );
        }

        public int GetHashCode( AlphaSymbolInfo obj )
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode( obj.Symbol );
        }
    }

    public PriceEntry? LatestPrice { get; set; }
    public List<PriceEntry> HistoricalPrices { get; } = [];
}