﻿using System.Collections.Generic;
using System.Threading;

namespace J4JSoftware.AlphaVantageRetriever;

public interface IAlphaVantageConnector
{
    string? ApiKey { get; }
    KeyValidationState KeyValidationState { get; }
    bool CallAvailable();

    IAsyncEnumerable<TAlpha> GetDataAsync<TAlpha>( string url, CancellationToken ctx )
        where TAlpha : class;
}
