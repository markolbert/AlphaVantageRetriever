using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace J4JSoftware.AlphaVantageRetriever;

internal static partial class LogExtensions
{
    [ LoggerMessage( LogLevel.Error, "{caller}: no retrievers are defined" ) ]
    public static partial void NoRetrievers( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Warning, "{caller}: multiple retrievers for {retriever}, first will be used" ) ]
    public static partial void MultipleRetrievers(
        this ILogger logger,
        string retriever,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Warning,
                     "{caller}: retrieval function {function} specified for multiple retrievers, first will be used" ) ]
    public static partial void OverlappingFunctions(
        this ILogger logger,
        string function,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Error, "{caller}: no parser defined for {retriever}" ) ]
    public static partial void NoParser(
        this ILogger logger,
        string retriever,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Warning, "{caller}: multiple parser defined for {retriever}, first will be used" ) ]
    public static partial void MultipleParsers(
        this ILogger logger,
        string retriever,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Error, "{caller}: Invalid setup, check appConfig.json file and contact developer" ) ]
    public static partial void InvalidSetup( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Warning, "{caller}: no tickers defined, nothing to retrieve" ) ]
    public static partial void NoTickersDefined( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Error, "{caller}: no API key defined, cannot access AlphaVantage server" ) ]
    public static partial void NoApiKey( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Error, "{caller}: no valid API key defined, cannot access AlphaVantage server" ) ]
    public static partial void NoValidApiKey( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Error, "{caller}: no parser defined for {dataSet}" ) ]
    public static partial void NoParserDefined(
        this ILogger logger,
        string dataSet,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Warning, "{caller}: No dataset specified, defaulting to retrieving ticker information" ) ]
    public static partial void NoDataSets( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Warning, "{caller}: premium account required for {retriever}" ) ]
    public static partial void PremiumAccountRequired(
        this ILogger logger,
        string retriever,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Warning, "{caller}: call limited reached, try again later" ) ]
    public static partial void CallLimitReached( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Error, "{caller}: query failed, AlphaVantage returned '{error}'" ) ]
    public static partial void QueryError( this ILogger logger, string error, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Error, "{caller}: query failed, exception was '{exMesg}'" ) ]
    public static partial void QueryFailed(
        this ILogger logger,
        string exMesg,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Error, "{caller}: function {function} not supported" ) ]
    public static partial void FunctionNotSupported(
        this ILogger logger,
        string function,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Information, "{caller}: retrieval started" ) ]
    public static partial void RetrievalStarted( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Information, "{caller}: Retrieving {function} for..." ) ]
    public static partial void RetrievingFunction(
        this ILogger logger,
        string function,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Information, "\t\t{caller}: for {ticker}..." ) ]
    public static partial void RetrievingTicker(
        this ILogger logger,
        string ticker,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Information, "{caller}: retrieval complete" ) ]
    public static partial void RetrievalComplete( this ILogger logger, [ CallerMemberName ] string caller = "" );

    [ LoggerMessage( LogLevel.Information, "\t\t{caller}: writing file {filename}..." ) ]
    public static partial void OutputFileStarted(
        this ILogger logger,
        string filename,
        [ CallerMemberName ] string caller = ""
    );

    [ LoggerMessage( LogLevel.Information, "\t\t{caller}: done" ) ]
    public static partial void OutputFileCompleted(
        this ILogger logger,
        [ CallerMemberName ] string caller = ""
    );
}
