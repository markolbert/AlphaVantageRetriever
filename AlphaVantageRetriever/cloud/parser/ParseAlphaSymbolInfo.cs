using Microsoft.Extensions.Logging;

namespace J4JSoftware.AlphaVantageRetriever;

public class ParseAlphaSymbolInfo(
    IAlphaVantageConnector alphaConnector,
    Configuration config,
    ILoggerFactory? loggerFactory
)
    : ParseAlphaBase<AlphaSymbolInfo, AlphaSymbolInfo>( alphaConnector, config, loggerFactory )
{
    public override bool SupportsFunction( AlphaVantageData toCheck ) => toCheck == AlphaVantageData.Ticker;

    protected override AlphaSymbolInfo Convert( AlphaVantageData avData, string ticker, AlphaSymbolInfo toConvert ) => toConvert;

    protected override string GetFunctionName( AlphaVantageData avData ) => "SYMBOL_SEARCH";
    protected override string GetTickerParameterName( AlphaVantageData avData ) => "keywords";
}
