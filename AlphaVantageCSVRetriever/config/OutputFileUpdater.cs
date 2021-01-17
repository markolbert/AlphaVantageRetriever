using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.ConsoleUtilities;
using J4JSoftware.Logging;

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class OutputFileUpdater : PropertyUpdater<string>
    {
        public OutputFileUpdater( IJ4JLogger? logger ) 
            : base( logger )
        {
        }

        public override UpdaterResult Validate( string? origValue, out string? newValue )
        {
            newValue = origValue;

            if( !string.IsNullOrEmpty( origValue ) && File.Exists( origValue ) )
                return UpdaterResult.OriginalOkay;

            var filePath = GetText( origValue ?? "**undefined**", 
                "output file path",
                Configuration.GetDefaultOutputFile() );

            if( string.IsNullOrEmpty( origValue ) || !File.Exists( origValue ) )
                return UpdaterResult.InvalidUserInput;

            newValue = filePath;

            return UpdaterResult.Changed;
        }
    }
}
