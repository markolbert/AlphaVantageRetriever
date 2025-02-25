using Microsoft.Extensions.Logging;

namespace J4JSoftware.AlphaVantageRetriever;

public class ParseAlphaLatestQuoteInfo(
    IAlphaVantageConnector alphaConnector,
    Configuration config,
    ILoggerFactory? loggerFactory
)
    : ParseAlphaBase<AlphaLatestQuoteData, AlphaLatestQuoteData>( alphaConnector, config, loggerFactory )
{
    public override bool SupportsFunction( AlphaVantageData toCheck ) => toCheck == AlphaVantageData.LatestPrice;

    protected override AlphaLatestQuoteData Convert( AlphaVantageData avData, string ticker, AlphaLatestQuoteData toConvert ) => toConvert;

    protected override string GetFunctionName( AlphaVantageData avData ) => "GLOBAL_QUOTE";
}
