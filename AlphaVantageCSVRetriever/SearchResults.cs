#pragma warning disable 8618

namespace J4JSoftware.AlphaVantageCSVRetriever
{
    public class SearchResults
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Currency { get; set; }
        public decimal MatchScore { get; set; }
    }
}