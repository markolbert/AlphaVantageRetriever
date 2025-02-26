using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace J4JSoftware.AlphaVantageRetriever;

public abstract class ParseAlphaBase<TOut, TIn>(
    IAlphaVantageConnector alphaConnector,
    Configuration config,
    ILoggerFactory? loggerFactory
) : IParseAlpha
    where TOut : class
    where TIn : class
{
    protected Configuration Configuration { get; } = config;
    protected ILogger? Logger { get; } = loggerFactory?.CreateLogger<ParseAlphaBase<TOut, TIn>>();

    public abstract bool SupportsFunction( AlphaVantageData toCheck );

    // by default, nothing requires a premium account so everything is supported. override in
    // derived classes as appropriate
    public virtual bool TryGetAccessAllowed( AlphaVantageData toCheck, out bool isAllowed )
    {
        isAllowed = true;
        return SupportsFunction( toCheck );
    }

    public async IAsyncEnumerable<TOut> ParseAlphaResponseAsync(
        AlphaVantageData avData,
        [ EnumeratorCancellation ] CancellationToken ctx
    )
    {
        if( Configuration.Tickers.Count == 0 )
        {
            Logger?.NoTickersDefined();
            yield break;
        }

        if( !alphaConnector.CallAvailable() )
        {
            Logger?.CallLimitReached();
            yield break;
        }

        if( !SupportsFunction( avData ) )
        {
            Logger?.FunctionNotSupported( avData.ToString() );
            yield break;
        }

        foreach( var ticker in Configuration.Tickers )
        {
            if( !alphaConnector.CallAvailable() || alphaConnector.KeyValidationState == KeyValidationState.NoValidKeys )
                yield break;

            // create the url, replacing everything in the template url
            // EXCEPT for @apiKey@, which is replaced by the IAlphaConnector
            var url = Configuration.AppConfiguration.UrlTemplate
                                   .Replace( "@function@",
                                             GetFunctionName( avData ),
                                             StringComparison.OrdinalIgnoreCase )
                                   .Replace( "@tickerParam@",
                                             GetTickerParameterName( avData ),
                                             StringComparison.OrdinalIgnoreCase )
                                   .Replace( "@ticker@", ticker, StringComparison.OrdinalIgnoreCase );

            Logger?.RetrievingTicker( ticker );

            await foreach( var item in alphaConnector.GetDataAsync<TIn>( url, ctx ) )
            {
                yield return Convert( avData, ticker, item );
            }
        }
    }

    protected abstract string GetFunctionName( AlphaVantageData avData );
    protected virtual string GetTickerParameterName( AlphaVantageData avData ) => "symbol";

    protected abstract TOut Convert( AlphaVantageData avData, string ticker, TIn toConvert );

    IAsyncEnumerable<object> IParseAlpha.GetData(
        AlphaVantageData avData,
        CancellationToken ctx
    ) =>
        ParseAlphaResponseAsync( avData, ctx );
}
