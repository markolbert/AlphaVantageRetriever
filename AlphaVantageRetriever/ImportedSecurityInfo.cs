using System;

namespace J4JSoftware.AlphaVantageRetriever
{
    public class ImportedSecurityInfo
    {
        public string Cusip { get; set; }
        public string SecurityName { get; set; }
        public string Type { get; set; }
        public string Issuer { get; set; }
        public string ClassSeries { get; set; }
        public string FPPCReportable { get; set; }
        public string Ticker { get; set; }
        public bool Reportable => FPPCReportable.Equals( "Y", StringComparison.OrdinalIgnoreCase );
    }
}
