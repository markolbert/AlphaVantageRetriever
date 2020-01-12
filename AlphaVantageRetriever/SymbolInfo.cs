using System;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class SymbolInfo
    {
        public string Issuer { get; set; }
        public string Cusip { get; set; }
        public string Ticker { get; set; }
        public string Category { get; set; }
        public string FPPCReportable { get; set; }
        public bool Reportable => FPPCReportable.Equals( "Y", StringComparison.OrdinalIgnoreCase );
    }
}
