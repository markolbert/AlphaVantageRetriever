namespace J4JSoftware.AlphaVantageRetriever;

public class AppConfiguration
{
    public ApiLimit DefaultLimits { get; set; } = new();
    public string UrlTemplate { get; set; } = string.Empty;
}
