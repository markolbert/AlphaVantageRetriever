using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Logging;
#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
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
