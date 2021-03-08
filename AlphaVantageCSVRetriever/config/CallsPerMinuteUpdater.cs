using J4JSoftware.ConsoleUtilities;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class CallsPerMinuteUpdater : PropertyUpdater<float>
    {
        public CallsPerMinuteUpdater( IJ4JLogger? logger )
            : base( logger )
        {
        }

        public override UpdaterResult Update( float origValue, out float newValue )
        {
            newValue = origValue;

            if( origValue > 0 )
                return UpdaterResult.OriginalOkay;

            newValue = Prompters.GetSingleValue( origValue, "calls per minute", 4.5F );

            return newValue > 0F ? UpdaterResult.Changed : UpdaterResult.InvalidUserInput;
        }
    }
}