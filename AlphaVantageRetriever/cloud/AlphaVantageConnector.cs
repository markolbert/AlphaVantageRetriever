using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace J4JSoftware.AlphaVantageRetriever;

public class AlphaVantageConnector( Configuration config, IDataProtector protector, ILoggerFactory? loggerFactory )
    : IAlphaVantageConnector
{
    private readonly ILogger? _logger = loggerFactory?.CreateLogger<AlphaVantageConnector>();

    public string? ApiKey { get; private set; }
    public KeyValidationState KeyValidationState { get; private set; } = KeyValidationState.Unknown;

    public bool CallAvailable()
    {
        var dtNow = DateTime.Now;

        var intervalSpan = dtNow - ( config.UserConfiguration.ApiCallHistory.IntervalStart ?? dtNow );

        if( intervalSpan > config.UserConfiguration.ApiLimit.IntervalTimeSpan )
            return true;

        return config.UserConfiguration.ApiCallHistory.NumCalls < config.UserConfiguration.ApiLimit.MaxRequests;
    }

    public async IAsyncEnumerable<TAlpha> GetDataAsync<TAlpha>( string url, CancellationToken ctx )
        where TAlpha : class
    {
        if( KeyValidationState == KeyValidationState.NoValidKeys )
        {
            _logger?.NoValidApiKey();
            yield break;
        }

        List<TAlpha>? parsedData = null;

        if( KeyValidationState == KeyValidationState.Validated )
            parsedData = await GetParsedData<TAlpha>( ApplyApiKey( url, ApiKey! ), ctx );

        // the only way to tell if we have a valid API key is to
        // try what we have and/or were given and see what happens
        if( parsedData == null && !string.IsNullOrEmpty( config.ApiKey ) )
        {
            // start by trying the command line API key, if we were given one
            parsedData = await GetParsedData<TAlpha>( ApplyApiKey( url, config.ApiKey ), ctx );

            // if we got data back, the API key is valid, so store it
            // for future use
            if( parsedData != null )
            {
                ApiKey = config.ApiKey;
                KeyValidationState = KeyValidationState.Validated;

                // also store it in the user config file for future invocations
                config.UserConfiguration.EncryptedApiKey = protector.Protect( config.ApiKey );
            }
        }

        if( parsedData == null && !string.IsNullOrEmpty( config.UserConfiguration.EncryptedApiKey ) )
        {
            var decryptedKey = protector.Unprotect( config.UserConfiguration.EncryptedApiKey );

            parsedData = await GetParsedData<TAlpha>( ApplyApiKey( url, decryptedKey ), ctx );

            // if we got data back, store the user config key for later use
            if( parsedData != null )
            {
                ApiKey = decryptedKey;
                KeyValidationState = KeyValidationState.Validated;
            }
            else
            {
                _logger?.NoValidApiKey();
                KeyValidationState = KeyValidationState.NoValidKeys;
            }
        }

        foreach( var item in parsedData ?? Enumerable.Empty<TAlpha>() )
        {
            yield return item;
        }
    }

    private string ApplyApiKey( string url, string apiKey ) =>
        url.Replace( "@apiKey@", config.ApiKey, StringComparison.OrdinalIgnoreCase );

    private async Task<List<TAlpha>?> GetParsedData<TAlpha>(
        string url,
        CancellationToken ctx
    )
        where TAlpha : class
    {
        string? text = null;

        try
        {
            text = await url.GetStringFromUrlAsync( token: ctx );
            RecordCall();

            return text.FromCsv<List<TAlpha>>();
        }
        catch( Exception ex )
        {
            if( text?.Contains( "error", StringComparison.OrdinalIgnoreCase ) ?? false )
                _logger?.QueryError( text );
            else _logger?.QueryFailed( ex.Message );

            return null;
        }
    }

    private void RecordCall()
    {
        var callTime = DateTime.Now;

        config.UserConfiguration.ApiCallHistory.LastCall = callTime;

        var intervalSpan = callTime - ( config.UserConfiguration.ApiCallHistory.IntervalStart ?? callTime );

        if( intervalSpan > config.UserConfiguration.ApiLimit.IntervalTimeSpan )
            config.UserConfiguration.ApiCallHistory.IntervalStart = callTime;
    }
}
