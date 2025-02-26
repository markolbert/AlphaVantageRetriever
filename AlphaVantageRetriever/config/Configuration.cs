using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace J4JSoftware.AlphaVantageRetriever;

public class Configuration
{
    [JsonIgnore]
    public string? ApiKey { get; set; }

    [JsonIgnore]
    public List<string> Tickers { get; set; } = [];

    [ JsonIgnore ]
    public List<AlphaVantageData> DataToRetrieve { get; set; } = [];

    public AppConfiguration AppConfiguration { get; set; } = new();
    public UserConfiguration UserConfiguration { get; set; } = new();

    public OutputFormat OutputFormat { get; set; } = OutputFormat.Json;

    public string? OutputFilePath { get; set; }
}