using System.Collections.Generic;
using System.Threading;

namespace J4JSoftware.AlphaVantageRetriever;

public interface IParseAlphaText
{
    bool SupportsFunction( AlphaVantageData toCheck );
    bool TryGetAccessAllowed( AlphaVantageData toCheck, out bool isAllowed );

    IAsyncEnumerable<object> GetData(
        AlphaVantageData avData,
        CancellationToken ctx
    );
}

public interface IParseAlpha<out TOut> : IParseAlphaText
    where TOut : class
{
    public IAsyncEnumerable<TOut> ParseAlphaResponseAsync(AlphaVantageData avData, CancellationToken ctx);
}
