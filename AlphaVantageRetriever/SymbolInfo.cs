using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaVantageRetriever
{
    public class SymbolInfo
    {
        public string Issuer { get; set; }
        public string Ticker { get; set; }
        public string Category { get; set; }
        public string FPPCReportable { get; set; }
        public bool Reportable => FPPCReportable.Equals( "Y", StringComparison.OrdinalIgnoreCase );
    }
}
