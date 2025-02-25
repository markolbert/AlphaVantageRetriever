using System;

namespace J4JSoftware.AlphaVantageRetriever;

public class ApiCallHistory
{
    public DateTime? IntervalStart { get; set; }
    public DateTime? LastCall { get; set; }
    public int NumCalls { get; set; }
}
