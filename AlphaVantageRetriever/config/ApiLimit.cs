using System;
using System.Text.Json.Serialization;

namespace J4JSoftware.AlphaVantageRetriever;

public class ApiLimit
{
    public LimitInterval Interval { get; set; } = LimitInterval.Day;
    public int MaxRequests { get; set; } = 25;

    [ JsonIgnore ]
    public TimeSpan IntervalTimeSpan
    {
        get
        {
            return Interval switch
            {
                LimitInterval.Minute => TimeSpan.FromMinutes( 1 ),
                LimitInterval.Hour => TimeSpan.FromHours( 1 ),
                _ => TimeSpan.FromDays( 1 )
            };
        }
    }
}
