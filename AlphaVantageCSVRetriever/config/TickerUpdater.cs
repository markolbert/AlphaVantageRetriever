using System.Collections.Generic;
using J4JSoftware.ConsoleUtilities;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class TickerUpdater : PropertyUpdater<List<string>>
    {
        public TickerUpdater( IJ4JLogger? logger ) 
            : base( logger )
        {
        }

        public override UpdaterResult Validate( List<string>? origValue, out List<string>? newValue )
        {
            newValue = origValue;

            if( origValue?.Count > 0 )
                return UpdaterResult.OriginalOkay;

            newValue = GetMultipleValues( origValue ?? new List<string>(), "ticker symbols" );

            return newValue.Count > 0 ? UpdaterResult.Changed : UpdaterResult.InvalidUserInput;
        }
    }
}