namespace J4JSoftware.AlphaVantageRetriever;

public class UserConfiguration
{
    public ApiLimit ApiLimit { get; set; } = new();
    public ApiCallHistory ApiCallHistory { get; set; } = new();
    public string? EncryptedApiKey { get; set; }
}
