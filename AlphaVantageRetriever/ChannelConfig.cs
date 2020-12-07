using System.Collections.Generic;
using J4JSoftware.Logging;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageRetriever
{
    public class ChannelConfig : LogChannels
    {
        public FileConfig File { get; set; }
        public ConsoleConfig Console { get; set; }

        public override IEnumerator<IChannelConfig> GetEnumerator()
        {
            yield return File;
            yield return Console;
        }
    }
}
