using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace J4JSoftware.AlphaVantageRetriever;

public class ParseAlphaPriceInfo(
    IAlphaVantageConnector alphaConnector,
    Configuration config,
    ILoggerFactory? loggerFactory
)
    : ParseAlphaBase<PriceEntry, AlphaPriceData>( alphaConnector, config, loggerFactory )
{
    public override bool SupportsFunction( AlphaVantageData toCheck ) =>
        toCheck switch
        {
            AlphaVantageData.Daily => true,
            AlphaVantageData.DailyAdjusted => true,
            AlphaVantageData.Weekly => true,
            AlphaVantageData.WeeklyAdjusted => true,
            AlphaVantageData.Monthly => true,
            AlphaVantageData.MonthlyAdjusted => true,
            _ => false
        };

    public override bool TryGetAccessAllowed( AlphaVantageData toCheck, out bool isAllowed )
    {
        isAllowed = toCheck switch
        {
            AlphaVantageData.DailyAdjusted => Configuration.UserConfiguration.ApiLimit.Interval != LimitInterval.Day,
            _ => true
        };

        return SupportsFunction( toCheck );
    }

    // TimeSpan will always be defined before this method is called
    protected override PriceEntry Convert( AlphaVantageData avData, string ticker, AlphaPriceData toConvert )
    {
        var timeSpan = avData switch
        {
            AlphaVantageData.Daily => TimeFrame.Daily,
            AlphaVantageData.DailyAdjusted => TimeFrame.Daily,
            AlphaVantageData.Weekly => TimeFrame.Weekly,
            AlphaVantageData.WeeklyAdjusted => TimeFrame.Weekly,
            AlphaVantageData.Monthly => TimeFrame.Monthly,
            AlphaVantageData.MonthlyAdjusted => TimeFrame.Monthly,
            _ => throw new InvalidEnumArgumentException( $"Unsupported {nameof( AlphaVantageData )} value '{avData}'" )
        };

        return new PriceEntry( ticker, toConvert, timeSpan );
    }

    protected override string GetFunctionName( AlphaVantageData avData ) =>
        avData switch
        {
            AlphaVantageData.Daily => "TIME_SERIES_DAILY",
            AlphaVantageData.DailyAdjusted => "TIME_SERIES_DAILY_ADJUSTED",
            AlphaVantageData.Weekly => "TIME_SERIES_WEEKLY",
            AlphaVantageData.WeeklyAdjusted => "TIME_SERIES_WEEKLY_ADJUSTED",
            AlphaVantageData.Monthly => "TIME_SERIES_MONTHLY",
            AlphaVantageData.MonthlyAdjusted => "TIME_SERIES_MONTHLY_ADJUSTED",
            _ => "unsupported!"
        };
}
