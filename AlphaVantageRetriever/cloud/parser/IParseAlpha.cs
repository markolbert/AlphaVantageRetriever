﻿using System.Collections.Generic;
using System.Threading;

namespace J4JSoftware.AlphaVantageRetriever;

public interface IParseAlpha
{
    bool SupportsFunction( AlphaVantageData toCheck );
    bool TryGetAccessAllowed( AlphaVantageData toCheck, out bool isAllowed );

    IAsyncEnumerable<object> GetData(
        AlphaVantageData avData,
        CancellationToken ctx
    );
}
