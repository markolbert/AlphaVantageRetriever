using J4JSoftware.ConsoleUtilities;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class ApiKeyUpdater : PropertyUpdater<string>
    {
        public ApiKeyUpdater( IJ4JLogger? logger )
            : base( logger )
        {
        }

        public override UpdaterResult Update( string? origValue, out string? newValue )
        {
            newValue = origValue;

            if( !string.IsNullOrEmpty( origValue ) )
                return UpdaterResult.OriginalOkay;

            var apiKey = Prompters.GetSingleValue( origValue ?? "**undefined**",
                "AlphaVantage API Key" );

            newValue = apiKey;

            return string.IsNullOrEmpty( newValue ) ? UpdaterResult.InvalidUserInput : UpdaterResult.Changed;
        }
    }
}