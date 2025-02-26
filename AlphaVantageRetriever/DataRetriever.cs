using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ServiceStack;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace J4JSoftware.AlphaVantageRetriever;

public class DataRetriever : IHostedService
{
    private readonly Configuration _config;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly bool _validSetup;
    private readonly List<IParseAlphaText> _alphaParsers;
    private readonly ILogger? _logger;

    public DataRetriever(
        Configuration config,
        IEnumerable<IParseAlphaText> alphaParsers,
        IHostApplicationLifetime lifetime,
        ILoggerFactory? loggerFactory )
    {
        _config = config;
        _lifetime = lifetime;
        _alphaParsers = alphaParsers.ToList();

        _logger = loggerFactory?.CreateLogger(GetType());
        _validSetup = CheckParsers();
    }

    private bool CheckParsers()
    {
        var retVal = true;

        foreach( var dataset in Enum.GetValues<AlphaVantageData>() )
        {
            switch( _alphaParsers.Count( p => p.SupportsFunction( dataset ) ) )
            {
                case 0:
                    _logger?.NoParser(dataset.ToString());
                    retVal = false;

                    break;

                case 1:
                    // no op; expected case
                    break;

                default:
                    _logger?.MultipleParsers( dataset.ToString() );
                    retVal = false;

                    break;
            }
        }

        return retVal;
    }

    public async Task StartAsync(CancellationToken ctx)
    {
        if( !_validSetup )
        {
            _logger?.InvalidSetup();
            _lifetime.StopApplication();
            return;
        }

        if (_config.Tickers.Count == 0)
        {
            _logger?.NoTickersDefined();
            _lifetime.StopApplication();
            return;
        }

        if ( string.IsNullOrEmpty( _config.UserConfiguration.EncryptedApiKey ) && string.IsNullOrEmpty( _config.ApiKey ) )
        {
            _logger?.NoApiKey();
            _lifetime.StopApplication();
            return;
        }

        if( _config.DataToRetrieve.Count == 0 )
        {
            _config.DataToRetrieve.Add( AlphaVantageData.Ticker );
            _logger?.NoDataSets();
        }

        _logger?.RetrievalStarted();

        foreach (var avData in _config.DataToRetrieve)
        {
            var parser = _alphaParsers.FirstOrDefault(r => r.SupportsFunction(avData));

            if (parser == null)
            {
                _logger?.NoParser( avData.ToString() );
                continue;
            }

            if( !parser.TryGetAccessAllowed( avData, out var isAllowed ) || !isAllowed )
            {
                _logger?.PremiumAccountRequired( avData.ToString() );
                continue;
            }

            _logger?.RetrievingFunction(avData.ToString());

            await OutputFile( avData, parser.GetData( avData, ctx ), ctx );
        }

        _logger?.RetrievalComplete();

        _lifetime.StopApplication();
    }

    private async Task OutputFile<T>(AlphaVantageData avData, IAsyncEnumerable<T> asyncData, CancellationToken ctx)
        where T : class
    {
        var path = Path.GetDirectoryName(_config.OutputFilePath) ?? Environment.CurrentDirectory;
        var fileName = Path.GetFileName(_config.OutputFilePath) ?? "AlphaVantage";
        var woExt = Path.GetFileNameWithoutExtension(fileName);

        var revisedFileName = $"{woExt}-{avData.ToString()}.{_config.OutputFormat switch
        {
            OutputFormat.Json => "json",
            _ => "csv"
        }}";

        var filePath = Path.Combine(path, revisedFileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        var data = await asyncData.ToListAsync( ctx );

        switch (_config.OutputFormat)
        {
            case OutputFormat.Json:
                _logger?.OutputFileStarted( revisedFileName );

                var options = new JsonSerializerOptions { WriteIndented = true };

                await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(data, options), ctx);

                _logger?.OutputFileCompleted();


                break;

            case OutputFormat.Csv:
                _logger?.OutputFileStarted(revisedFileName);

                await File.WriteAllTextAsync(filePath, data.ToCsv(), ctx);

                _logger?.OutputFileCompleted();

                break;

            default:
                _logger?.LogError("Unsupported output style {style}", _config.OutputFormat.ToString());
                break;
        }
    }

    public async Task StopAsync( CancellationToken ctx )
    {
        await Log.CloseAndFlushAsync();

        var userConfigPath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                                           "J4JSoftware",
                                           nameof( AlphaVantageRetriever ),
                                           "userConfig.json" );

        if( File.Exists( userConfigPath ) )
            File.Delete( userConfigPath );

        var options = new JsonSerializerOptions { WriteIndented = true };

        await File.WriteAllTextAsync( userConfigPath,
                                      JsonSerializer.Serialize( _config.UserConfiguration, options ),
                                      ctx );
    }
}